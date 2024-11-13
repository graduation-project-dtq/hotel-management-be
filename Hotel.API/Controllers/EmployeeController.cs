using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.DTOs.EmployeeDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index =1, int pageSize =10 , string? idSearch = null, string? nameSearch = null
           , string? email = null, string? phone = null, DateOnly? dateOfBirth = null, DateOnly? hireDate = null)
        {
            PaginatedList<GetEmployeeDTO> result= await _employeeService.GetPageAsync(index, pageSize, idSearch,
                nameSearch, email, phone, dateOfBirth, hireDate);
            return Ok(new BaseResponse<PaginatedList<GetEmployeeDTO>>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Lấy danh sách nhân viên thành công!"
            ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeAsync(CreateEmployeeDTO createEmployeeDTO)
        {
            GetEmployeeDTO result = await _employeeService.CreateEmployeeAsync(createEmployeeDTO);
            return Ok(new BaseResponse<GetEmployeeDTO>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Tạo nhân viên thành công"
            ));
        }
    }
}
