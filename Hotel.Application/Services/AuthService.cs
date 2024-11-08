using AutoMapper;
using Hotel.Application.DTOs.UserDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
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
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly ICustomerService _customerService;
        private readonly IEmployeeService _employeeService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuthService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AuthService> logger,
            ICustomerService customerService,
            IEmployeeService employeeService,
            ITokenService tokenService,
            IEmailService emailService,
            IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _customerService = customerService;
            _employeeService = employeeService;
            _tokenService = tokenService;
            _emailService = emailService;
            _contextAccessor = contextAccessor;
        }

        private string currentUser => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        public async Task RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            Account? existAccount = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == registerRequestDto.Email && x.DeletedTime == null);
            if (existAccount != null)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Email đã tồn tại");
            }

            //Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(x => x.RoleName == registerRequestDto.RoleName && x.DeletedTime == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "The specified role was not found. Please provide a valid role.");

            Account account = _mapper.Map<Account>(registerRequestDto);
            
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            account.Password = passwordHasher.HashPassword(account, account.Password);
            account.RoleId = "c401bb08da484925900a63575c3717f8";
            account.IsActive = false;

            //Check SDT
            Customer ? exitCustomer = await _unitOfWork.GetRepository<Customer>().Entities.Where(c=>c.Phone.ToString()==registerRequestDto.Phone).FirstOrDefaultAsync();

            if(exitCustomer != null)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.DUPLICATE, "SDT đã tồn tại");

            }
            //Add Customer
            Customer customer = new Customer()
            {
                Id = account.Id,
                AccountID = account.Id,
                Name = account.Name ?? "Khách hàng",
                Email = account.Email,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                CredibilityScore = 100,
                AccumulatedPoints = 0,
                Phone = string.IsNullOrWhiteSpace(registerRequestDto.Phone) != true ? registerRequestDto.Phone : string.Empty
            };

            account.Code = randActiveCode();
            await _emailService.ActiveAccountEmailAsync( account.Code,  account.Email);
            await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

      
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == loginRequestDto.Email && x.DeletedTime == null) 
                ?? throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "Tài khoản hoặc mật khẩu không chính xác");
            //check status
            if (account.IsActive == false)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "Tài khoản chưa được kích hoạt");
            }

            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            string hashedInputPassWord = passwordHasher.HashPassword(null, loginRequestDto.Password);
            if (hashedInputPassWord != account.Password)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Sai tài khoản hoặc mật khẩy");
            }
            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(x => x.Id == account.RoleId && x.DeletedTime == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found for the account");
            string roleName = role.RoleName;
            TokenResponseDto tokenResponseDto = _tokenService.GenerateToken(account, roleName);
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                TokenResponse = tokenResponseDto,
            };
            return loginResponseDto;
        }

        public async Task<TokenResponseDto> RefreshAccessTokenAsync(RefeshTokenRequestDto refeshTokenRequest)
        {
            if (string.IsNullOrWhiteSpace(refeshTokenRequest.RefreshToken))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập dữ liệu hợp lệ");
            }
            return await _tokenService.RefreshAccessToken(refeshTokenRequest);
        }
        public string randActiveCode()
        {
            var random = new Random();
            int randomNumber = random.Next(100000, 1000000); // Tạo số ngẫu nhiên từ 100000 đến 999999
            return randomNumber.ToString("D6"); // Định dạng chuỗi 6 chữ số
        }
        public async Task ActiveAccountAsync(string email,string code)
        {
            if(string.IsNullOrWhiteSpace(email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống Email");
            }
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống mã xác thực");
            }

            Account account = await _unitOfWork.GetRepository<Account>().Entities.Where( a=>a.Email.Equals(email) && !a.DeletedTime.HasValue).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy tài khoản ngươi dùng");
            if(account.IsActive)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Tài khoản đã xác thực rồi");
            }    
            if(!code.Equals(account.Code))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Mã xác thực không chính xác");
            }

            account.IsActive = true;
            account.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ReponseCode(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống Email");
            }
            Account account = await _unitOfWork.GetRepository<Account>().Entities.Where(a => a.Email.Equals(email) && !a.DeletedTime.HasValue).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không tìm thấy tài khoản ngươi dùng");
            if(account.IsActive)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "Tài khoản đã được kích hoạt rồi");
            }
            //Tạo code mới
            account.Code = randActiveCode();
            account.LastUpdatedTime = CoreHelper.SystemTimeNow;

            //Gửi mail
            await _emailService.ActiveAccountEmailAsync(account.Code, email);
            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }

        //đăng nhập GG
        public async Task<LoginResponseDto> SignInWithGoogleAsync(GoogleSignInDto googleSignInDto)
        {
            // Kiểm tra xem tài khoản đã tồn tại chưa
            Account ? account = await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(x => x.Email == googleSignInDto.Email && x.DeletedTime == null);
               
            if (account == null)
            {
                // Nếu chưa có tài khoản, tạo tài khoản mới
                string randomPassword = GenerateRandomPassword();
                FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));

                account = new Account
                {
                    Email = googleSignInDto.Email,
                    Name = googleSignInDto.TenND ?? "Khách hàng",
                    IsActive = true, // Tài khoản Google đã được xác thực
                    RoleId = "c401bb08da484925900a63575c3717f8", // RoleId cho Customer
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow,
                    Password = passwordHasher.HashPassword(null, randomPassword),
                    Code = randActiveCode() // Tạo mã xác thực (có thể không cần thiết cho Google Sign-In)
                };

                // Tạo Customer tương ứng
                Customer customer = new Customer()
                {
                    Id = account.Id,
                    AccountID = account.Id,
                    Name = account.Name,
                    Email = account.Email,
                    CreatedTime = CoreHelper.SystemTimeNow,
                    LastUpdatedTime = CoreHelper.SystemTimeNow,
                    CredibilityScore = 100,
                    AccumulatedPoints = 0,
                };

                await _unitOfWork.GetRepository<Account>().InsertAsync(account);
                await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
                await _unitOfWork.SaveChangesAsync();
            }

            // Kiểm tra trạng thái tài khoản (có thể bỏ qua vì tài khoản Google luôn active)
            if (!account.IsActive)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "Tài khoản chưa được kích hoạt");
            }

            // Lấy thông tin role
            Role role = await _unitOfWork.GetRepository<Role>().Entities
                .FirstOrDefaultAsync(x => x.Id == account.RoleId && x.DeletedTime == null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found for the account");

            // Tạo token
            TokenResponseDto tokenResponseDto = _tokenService.GenerateToken(account, role.RoleName);

            // Tạo response
            LoginResponseDto loginResponseDto = new LoginResponseDto
            {
                TokenResponse = tokenResponseDto,
            };

            return loginResponseDto;
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task DeleteAccount(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống người dùng");
            }

            Account account=await _unitOfWork.GetRepository<Account>().Entities.Where(r => !r.DeletedTime.HasValue).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Người dùng không tồn tại");

            //Kiểm tra có phải tài khoản khách hàng hay không
            if (account.IsAdmin != true)
            {
                Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(account.Id)
                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tài khoản không tồn tại");

                //Xoá khách hàng
                customer.DeletedTime = CoreHelper.SystemTimeNow;
                customer.DeletedBy = currentUser;

                await _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
            }
            else 
            {
                Employee employee = await _unitOfWork.GetRepository<Employee>().GetByIdAsync(account.Id)
                 ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Tài khoản không tồn tại");

                //Xoá khách hàng
                employee.DeletedTime = CoreHelper.SystemTimeNow;
                employee.DeletedBy = currentUser;

                await _unitOfWork.GetRepository<Employee>().UpdateAsync(employee);
            }
            account.DeletedTime = CoreHelper.SystemTimeNow;
            account.DeletedBy = currentUser;

            await _unitOfWork.GetRepository<Account>().UpdateAsync(account);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
