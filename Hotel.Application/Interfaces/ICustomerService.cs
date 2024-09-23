
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO);
    }
}
