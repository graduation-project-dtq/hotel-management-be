using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<PaginatedList<GetCustomerDTO>> GetPageAsync(int index, int pageSize, string idSearch,
            string nameSearch, string phoneNumberSearch, string identityCardSearch);
        Task<GetCustomerDTO> CreateCustomerAsync(CreateCustomerDTO createCustomerDTO);
        Task UpdateCustomerAsync(string id,PutCustomerDTO model);
        Task<GetCustomerDTO> GetCustomerByEmailAsync(string email);
        Task DeleteCustomer(string id);
    }
}
