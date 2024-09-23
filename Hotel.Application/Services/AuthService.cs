﻿using AutoMapper;

using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.DTOs.UserDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;

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
        public AuthService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AuthService> logger,
            ICustomerService customerService,
            IEmployeeService employeeService,
            ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _customerService = customerService;
            _employeeService = employeeService;
            _tokenService = tokenService;
        }
        public async Task RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            Account? existAccount = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == registerRequestDto.Email && x.DeletedTime == null);
            if (existAccount != null)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "This email is already registered.");
            }

            Role role = await _unitOfWork.GetRepository<Role>().Entities.FirstOrDefaultAsync(x => x.RoleName == registerRequestDto.RoleName && x.DeletedTime == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "The specified role was not found. Please provide a valid role.");

            Account account = _mapper.Map<Account>(registerRequestDto);

            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            account.Password = passwordHasher.HashPassword(account, account.Password);
            account.RoleId = role.Id;
            account.IsActive = true;
           
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.SaveChangesAsync();

            await AssignRoleSpecificService(account.Id, registerRequestDto);
        }
      
        //Kiểm tra role và gọi service tương ứng
        private async Task AssignRoleSpecificService(string accountId, RegisterRequestDto registerRequestDto)
        {
            try
            {
                string roleName = registerRequestDto.RoleName;
                if (roleName == string.Empty)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Role name is required");
                }
                switch (roleName)
                {
                    case CLAIMS_VALUES.ROLE_TYPE.CUSTOMER:
                        CreateCustomerDTO createCustomer = new CreateCustomerDTO()
                        {
                            AccountId = accountId,
                            Name = registerRequestDto.Name,
                            Email = registerRequestDto.Email,
                            NumberPhone = registerRequestDto.NumberPhone,
                        };
                        await _customerService.CreateCustomerAsync(createCustomer);
                        break;
                    case CLAIMS_VALUES.ROLE_TYPE.ADMIN:
                    case CLAIMS_VALUES.ROLE_TYPE.EMPLOYEE:
                        CreateEmployeeDTO createReviewerDto = new CreateEmployeeDTO()
                        {
                            AccountId = accountId,
                            Name = registerRequestDto.Name,
                            NumberPhone = registerRequestDto.NumberPhone,
                        };
                        await _employeeService.CreateEmployeeAsync(createReviewerDto);
                        break;
                   
                    default:
                        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Role name is invalid");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when assign role specific service");
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "An internal server error occurred. Please try again later.");
            }
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            Account account = await _unitOfWork.GetRepository<Account>().Entities.FirstOrDefaultAsync(x => x.Email == loginRequestDto.Email && x.DeletedTime == null) ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.BADREQUEST, "Email or password is incorrect");
            //check status
            if (account.IsActive == false)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.BADREQUEST, "This account is not active");
            }

            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            string hashedInputPassWord = passwordHasher.HashPassword(null, loginRequestDto.Password);
            if (hashedInputPassWord != account.Password)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Email or password is incorrect");
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
            if (string.IsNullOrEmpty(refeshTokenRequest.RefreshToken))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Refresh token is required");
            }
            return await _tokenService.RefreshAccessToken(refeshTokenRequest);
        }
    }
}
