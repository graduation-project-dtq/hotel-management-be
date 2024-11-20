using AutoMapper;
using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hotel.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IEmailService _emailService;
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmployeeService> logger, 
            IHttpContextAccessor contextAccessor, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _emailService = emailService;
        }
        public async Task<PaginatedList<GetEmployeeDTO>> GetPageAsync(int index, int pageSize, string? idSearch, string? nameSearch
           , string? email, string? phone, DateOnly? dateOfBirth, DateOnly? hireDate)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }
            IQueryable<Employee> query = _unitOfWork.GetRepository<Employee>().Entities
                .Where(c => !c.DeletedTime.HasValue)
                .OrderByDescending(c => c.CreatedTime);
            //Tìm theo Id
            if(!string.IsNullOrEmpty(idSearch))
            {
                query=query.Where(e=>e.Id.Equals(idSearch));
            }
            //Tìm theo tên
            if (!string.IsNullOrEmpty(idSearch))
            {
                query = query.Where(e => e.Id.Contains(nameSearch != null ? nameSearch : string.Empty));
            }
            //Tìm theo email
            if (!string.IsNullOrEmpty(idSearch))
            {
                query = query.Where(e => e.Id.Equals(email != null ? email : string.Empty));
            }
            //Tìm theo SDT
            if (!string.IsNullOrEmpty(idSearch))
            {
                query = query.Where(e => e.Id.Equals(phone != null ? phone : string.Empty));
            }
            //Tìm theo ngày sinh
            if (!string.IsNullOrEmpty(idSearch))
            {
                query = query.Where(e => e.DateOfBirth.Equals(dateOfBirth));
            }
            //Tìm theo ngày vào làm
            if (!string.IsNullOrEmpty(idSearch))
            {
                query = query.Where(e => e.HireDate.Equals(hireDate));
            }

            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetEmployeeDTO>(new List<GetEmployeeDTO>(), totalCount, index, pageSize);
            }
            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            List<GetEmployeeDTO> responseItems = _mapper.Map<List<GetEmployeeDTO>>(resultQuery);
            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetEmployeeDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }
        public async Task<GetEmployeeDTO> CreateEmployeeAsync(CreateEmployeeDTO model)
        {
            //Kiểm tra trùng email
            Account? exitaccount = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(a => a.Email.Equals(model.Email) && !a.DeletedTime.HasValue);
            if (exitaccount != null)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Email đã tồn tại");
            }
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
       
            Role ? role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(r=>r.RoleName.Equals("Employee") && !r.DeletedTime.HasValue);
            Account account = new Account()
            {
                Email= model.Email,
                Name= model.Name,
                Password = passwordHasher.HashPassword(new Account(), "Admin123@"), 
                RoleId=role != null ?role.Id : string.Empty,
                IsActive=false,
                IsAdmin=true,
            };

            account.CreatedBy= currentUserId;
            account.LastUpdatedBy= currentUserId;
            
            account.Code = AuthService.randActiveCode();
            await _emailService.ActiveAccountEmailAsync(account.Code, account.Email);

            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.SaveChangesAsync();
            //Gửi mail để xác nhận

            //Tạo nhân viên
            Employee employee = new Employee()
            {
                Id = account.Id,
                AccountID = account.Id,
                Name=model.Name,
                Email=model.Email,
                Phone=model.Phone,
                IdentityCard=model.IdentityCard,
                Sex=model.Sex,
                DateOfBirth=model.DateOfBirth,
                Address=model.Address,
            };
          
            employee.HireDate= DateOnly.FromDateTime(DateTime.Now);
            employee.CreatedBy = currentUserId;
            employee.LastUpdatedBy = currentUserId;

            await _unitOfWork.GetRepository<Employee>().InsertAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            
            GetEmployeeDTO getEmployeeDTO = _mapper.Map<GetEmployeeDTO>(employee);
            return getEmployeeDTO;
        }
        //Cập nhật thông tin nhân viên
        public async Task<GetEmployeeDTO> UpdateEmployeeAsync(string id, PutEmployeeDTO model)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng chọn nhân viên");
            }
            // 1. Tìm nhân viên theo mã
            Employee employee = await _unitOfWork.GetRepository < Employee>()
                .Entities.FirstOrDefaultAsync(e=>e.Id.Equals(id) && !e.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Nhân viên không tồn tại");
            //2. Gán thông tin cần update
            employee = _mapper.Map<Employee>(model);

            employee.LastUpdatedBy=currentUserId;
            employee.LastUpdatedTime = CoreHelper.SystemTimeNow;

            //3. lưu và CSDL
            await _unitOfWork.GetRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            GetEmployeeDTO getEmployeeDTO= _mapper.Map<GetEmployeeDTO>(employee);
            return getEmployeeDTO;
        }
        //Xoá nhân viên
        public async Task DeleteEmployeeAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng chọn nhân viên");
            }
            // 1. Tìm nhân viên theo mã
            Employee employee = await _unitOfWork.GetRepository<Employee>()
                .Entities.FirstOrDefaultAsync(e => e.Id.Equals(id) && !e.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Nhân viên không tồn tại");
            employee.DeletedBy = currentUserId;
            employee.DeletedTime = CoreHelper.SystemTimeNow;

            //3. lưu và CSDL
            await _unitOfWork.GetRepository<Employee>().UpdateAsync(employee);
            await _unitOfWork.SaveChangesAsync();

            Account? account = await _unitOfWork.GetRepository<Account>().Entities
                .Where(a => a.Id.Equals(employee.AccountID) && !a.DeletedTime.HasValue).FirstOrDefaultAsync();
            
            if(account!= null)
            {
                account.DeletedBy = currentUserId;
                account.DeletedTime = CoreHelper.SystemTimeNow;

                await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
