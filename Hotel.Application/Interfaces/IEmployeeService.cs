using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.PaggingItems;

namespace Hotel.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<PaginatedList<GetEmployeeDTO>> GetPageAsync(int index, int pageSize, string? idSearch, string? nameSearch
           ,string? email, string? phone, DateOnly? dateOfBirth, DateOnly? hireDate);
        Task<GetEmployeeDTO> CreateEmployeeAsync(CreateEmployeeDTO createEmployeeDTO);
        Task<GetEmployeeDTO> UpdateEmployeeAsync(string id, PutEmployeeDTO model);
    }
}
