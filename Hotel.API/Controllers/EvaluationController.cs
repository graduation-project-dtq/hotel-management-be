﻿using Hotel.Application.DTOs.EvaluationDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly IEvaluationService _evaluationService;
        public EvaluationController(IEvaluationService evaluationService)
        {
            _evaluationService = evaluationService;
        }
        [HttpGet("getPage")]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "", string customerID = "", string roomTypeId = "")
        {
            PaginatedList<GetEvaluationDTO> result = await _evaluationService.GetPageAsync( index,  pageSize, idSearch, customerID, roomTypeId);
            return Ok(new BaseResponse<PaginatedList<GetEvaluationDTO>>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Lấy thông tin đánh giá thành công!"
           ));
        }
        [HttpGet("all")]
        public async Task<IActionResult>  GetEvaluationAsync(string roomTypeId)
        {
            List<GetEvaluationDTO> result = await _evaluationService.GetEvaluationAsync(roomTypeId);
            return Ok(new BaseResponse<List<GetEvaluationDTO>>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Lấy thông tin giá thành công!"
           ));
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles ="CUSTOMER")]
        public async Task<IActionResult> CreateEvaluationAsync([FromForm] List<IFormFile>? images, [FromForm] PostEvaluationDTO model)
        {
            await _evaluationService.CreateEvaluationAsync(images, model);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Đánh giá thành công!"
            ));
        }
        //Update đánh giá
        [HttpPut]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "CUSTOMER,ADMIN,EMPLOYEE")]
        public async Task<IActionResult> UpdateEvaluationAsync([FromForm]string id,[FromForm] List<IFormFile>? images, [FromForm] PutEvaluationDTO model)
        {
            await _evaluationService.UpdateEvaluationAsync(id,images, model);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Cập nhật thành công!"
            ));
        }

        //Xoá đánh giá
        [HttpDelete]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteEvaluationAsync(string id)
        {
            await _evaluationService.DeleteEvaluationAsync(id);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Xoá thành công!"
            ));
        }
    }
}
