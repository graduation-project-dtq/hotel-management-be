using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee> CreateEmployeeAsync(CreateEmployeeDTO createEmployeeDTO);
    }
}
