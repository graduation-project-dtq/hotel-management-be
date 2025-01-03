﻿using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.EvaluationDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Entities;
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


        [HttpGet]
     
        public async Task<IActionResult> GetPageAsync(int index =1, int pageSize = 10, string idSearch = "",
        string nameSearch = "", string phoneNumberSearch = "", string identityCardSearch = "")
        {
            PaginatedList<GetCustomerDTO> result = await _customerService.GetPageAsync(index, pageSize, idSearch, nameSearch ,phoneNumberSearch, identityCardSearch);
            return Ok(new BaseResponse<PaginatedList<GetCustomerDTO>>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Lấy danh sách khách hàng thành công!"
           ));
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
        [HttpPost]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult>  CreateCustomerAsync(CreateCustomerDTO model)
        {
            GetCustomerDTO result = await _customerService.CreateCustomerAsync(model);
            return Ok(new BaseResponse<GetCustomerDTO>(
            statusCode: StatusCodes.Status200OK,
            code: ResponseCodeConstants.SUCCESS,
            data: result,
            message: "Tạo khách hàng thành công"
         ));
        }
        /// <summary>
        /// Cập nhật thông tin của khách hàng
        /// </summary>
        /// <param name="email"></param>
        /// <param name="model"></param>

        [HttpPut("{email}")]
        [Authorize(Roles ="CUSTOMER")]
        public async Task<IActionResult> UpdateCustomerAsync(string email, PutCustomerDTO model)
        {
            await _customerService.UpdateCustomerAsync(email, model);
            return Ok(new BaseResponse<string ?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "",
                message: "Cập nhật thành công"
             ));
        }

        [HttpDelete]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            await _customerService.DeleteCustomer(id);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "",
                message: "Xoá thành công"
             ));
        }
    }
}
