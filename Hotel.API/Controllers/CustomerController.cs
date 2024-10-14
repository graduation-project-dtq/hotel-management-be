using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// Tìm khách hàng bằng Email
        /// </summary>
        /// <param name="email"></param>

        [HttpGet("{email}")]
        public async Task<IActionResult> GetCustomerByEmailAsync(string email)
        {
            GetCustomerDTO result =await _customerService.GetCustomerByEmailAsync(email);
            return Ok(new BaseResponse<GetCustomerDTO>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: result,
              message: "Lấy thông tin khách hàng thành công"
           ));
        }

        /// <summary>
        /// Cập nhật thông tin của khách hàng
        /// </summary>
        /// <param name="email"></param>
        /// <param name="model"></param>

        [HttpPut("{email}")]
        [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.CUSTOMER)]
        public async Task<IActionResult> UpdateCustomerAsync(string email, PutCustomerDTO model)
        {
            await _customerService.UpdateCustomerAsync(email, model);
            return Ok(new BaseResponse<string ?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "",
                message: "Sửa thông tin thành công"
             ));
        }
    }
}
