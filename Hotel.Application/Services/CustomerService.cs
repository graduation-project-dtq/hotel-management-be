﻿
using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.EvaluationDTO;
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
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
            Customer customer = await _unitOfWork.GetRepository<Customer>().Entities.Where(c => c.Email == email && c.DeletedTime == null).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy khách hàng có email là !" + email);

            GetCustomerDTO dto = _mapper.Map<GetCustomerDTO>(customer);
            return dto;
        }

        public async Task<PaginatedList<GetCustomerDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch, string phoneNumberSearch, string identityCardSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }
            IQueryable<Customer> query = _unitOfWork.GetRepository<Customer>().Entities
               .Where(e => !e.DeletedTime.HasValue)
               .OrderByDescending(c => c.CreatedTime);
            //Tìm theo Id
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
            }

            //Tìm theo id
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
            }

            //Tìm theo tên
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                query = query.Where(r => r.Name.Contains(nameSearch));
            }

            //Tìm theo SDT
            if (!string.IsNullOrWhiteSpace(phoneNumberSearch))
            {
                query = query.Where(r => r.Phone.Equals(phoneNumberSearch));
            }
            //Tìm theo CCCD
            if (!string.IsNullOrWhiteSpace(identityCardSearch))
            {
                query = query.Where(r => r.IdentityCard.Equals(identityCardSearch));
            }
            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetCustomerDTO>(new List<GetCustomerDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            List<GetCustomerDTO> responseItems = _mapper.Map<List<GetCustomerDTO>>(resultQuery);

            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetCustomerDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }
        public async Task<GetCustomerDTO> CreateCustomerAsync(CreateCustomerDTO model)
        {
            Customer ? exitCustomer = new Customer();
            if (!string.IsNullOrWhiteSpace(model.IdentityCard))
            {
                exitCustomer = await _unitOfWork.GetRepository<Customer>().Entities.Where(c => c.IdentityCard.Equals(model.IdentityCard)
                && !c.DeletedTime.HasValue).FirstOrDefaultAsync();

                if (exitCustomer != null)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Đã tồn tại khách hàng có số CCCD :"+model.IdentityCard);
                }
            }
            if (!string.IsNullOrWhiteSpace(model.NumberPhone))
            {
                exitCustomer = await _unitOfWork.GetRepository<Customer>().Entities.Where(c => c.Phone.Equals(model.NumberPhone)
                && !c.DeletedTime.HasValue).FirstOrDefaultAsync();

                if (exitCustomer != null)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Đã tồn tại khách hàng có số điện thoại :" + model.IdentityCard);
                }
            }
            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                exitCustomer = await _unitOfWork.GetRepository<Customer>().Entities.Where(c => c.Email.Equals(model.Email)
                && !c.DeletedTime.HasValue).FirstOrDefaultAsync();

                if (exitCustomer != null)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Đã tồn tại khách hàng có email :" + model.Email);
                }
            }
    
            Customer customer = _mapper.Map<Customer>(model);

            customer.AccumulatedPoints = 0;
            customer.CredibilityScore = 100; //Điểm uy tín 
            await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            GetCustomerDTO result = _mapper.Map<GetCustomerDTO>(customer);
            return result;
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
