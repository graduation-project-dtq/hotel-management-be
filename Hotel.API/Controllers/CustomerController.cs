﻿using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
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
        [HttpPut]
        public async Task<IActionResult> UpdateCustomerAsync(string id,PutCustomerDTO model)
        {
            await _customerService.UpdateCustomerAsync(id, model);
            return Ok(new BaseResponse<string ?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "",
                message: "Sửa thông tin thành công"
             ));
        }
    }
}