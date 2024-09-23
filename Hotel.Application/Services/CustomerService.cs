
using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Customer> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO)
        {
            Customer? existsAuthor = await _unitOfWork.GetRepository<Customer>().Entities.FirstOrDefaultAsync(a => a.AccountID == createCustomerDTO.AccountId);
            if (existsAuthor != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Author is existed!");
            }

            Customer customer = _mapper.Map<Customer>(createCustomerDTO);
            await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            return customer;
        }
    }
}
