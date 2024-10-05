using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO);
        Task UpdateCustomerAsync(string id,PutCustomerDTO model);
        Task<GetCustomerDTO> GetCustomerByEmailAsync(string email);
    }
}
