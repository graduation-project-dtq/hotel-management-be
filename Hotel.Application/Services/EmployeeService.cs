using AutoMapper;
using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmployeeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
    
        public async Task<Employee> CreateEmployeeAsync(CreateEmployeeDTO createEmployeeDTO)
        {
            Customer? existsAuthor = await _unitOfWork.GetRepository<Customer>().Entities.FirstOrDefaultAsync(a => a.AccountID == createEmployeeDTO.AccountId);
            if (existsAuthor != null)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Author is existed!");
            }

            Employee employee = _mapper.Map<Employee>(createEmployeeDTO);
            await _unitOfWork.GetRepository<Employee>().InsertAsync(employee);
            await _unitOfWork.SaveChangesAsync();
            return employee;
        }
    }
}
