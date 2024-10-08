using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> CreateEmployeeAsync(CreateEmployeeDTO createEmployeeDTO);
    }
}
