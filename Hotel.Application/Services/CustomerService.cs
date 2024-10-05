
using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Hotel.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger,IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }
        public async Task<Customer> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO)
        {
            Customer? existsCustomer = await _unitOfWork.GetRepository<Customer>().Entities.FirstOrDefaultAsync(a => a.AccountID == createCustomerDTO.AccountId);
            if (existsCustomer != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Khacsh hang");
            }

            Customer customer = _mapper.Map<Customer>(createCustomerDTO);

            customer.CreatedTime=customer.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return customer;
        }

        public async Task<GetCustomerDTO> GetCustomerByEmailAsync(string email)
        {
            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Email không được để trống!");
            }
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailRegex))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Email không hợp lệ!");
            }
            Customer customer =await _unitOfWork.GetRepository<Customer>().Entities.Where(c=>c.Email == email && c.DeletedTime==null).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy khách hàng có email là !"+email);

            GetCustomerDTO dto= _mapper.Map<GetCustomerDTO>(customer);
            return dto;
        }
        public async Task UpdateCustomerAsync(string email, PutCustomerDTO model)
        {
            // Kiểm tra ID khách hàng
            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID rỗng hoặc không hợp lệ!");
            }
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailRegex))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Email không hợp lệ!");
            }
            // Kiểm tra tên khách hàng
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống tên!");
            }

            // Kiểm tra giới tính khách hàng
            if (String.IsNullOrWhiteSpace(model.Sex))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống giới tính!");
            }

            // Kiểm tra khách hàng có tồn tại không
            Customer customer = await _unitOfWork.GetRepository<Customer>().Entities.FirstOrDefaultAsync(c => c.Email == email)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy thông tin khách hàng");

            // Map các thông tin mới từ DTO vào entity
            customer.Name = model.Name;
            customer.IdentityCard = model.IdentityCard;
            customer.Sex = model.Sex;

            // Chuyển đổi từ DateOnly sang DateTime và đảm bảo thiết lập Kind
            if (model.DateOfBirth != default) // Kiểm tra nếu DateOfBirth có giá trị
            {
                // Chuyển đổi từ DateOnly sang DateTime
                DateTime dateTime = new DateTime(model.DateOfBirth.Year, model.DateOfBirth.Month, model.DateOfBirth.Day);
                customer.DateOfBirth = DateOnly.FromDateTime(dateTime);
            }
            else
            {
                customer.DateOfBirth = null; // Gán null nếu model.DateOfBirth không có giá trị
            }

            customer.Phone = model.Phone;
            customer.Address = model.Address;

            // Thiết lập thông tin người cập nhật và thời gian cập nhật
            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            customer.LastUpdatedTime = DateTime.UtcNow; // Lấy thời gian UTC
            customer.LastUpdatedBy = userID;

            // Cập nhật và lưu thay đổi vào DB
            await _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
