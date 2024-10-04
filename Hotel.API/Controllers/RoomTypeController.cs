﻿using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Base;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(IRoomTypeService roomTypeService, ILogger<RoomTypeController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }
        // Lấy tất cả RoomType
        [HttpGet]
        public async Task<IActionResult> GetAllRoomType()
        {
            var roomTypes = await _roomTypeService.GetAllRoomType();
            return Ok(new BaseResponseModel<List<GetRoomTypeDTO>>(
                   statusCode: StatusCodes.Status200OK,
                   code: "Lấy danh sách loại phòng thành công",
                   data: roomTypes
               ));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomTypeById(string id)
        {
            var roomType = await _roomTypeService.GetRoomTypeById(id);
            return Ok(new BaseResponseModel<GetRoomTypeDTO>(
                   statusCode: StatusCodes.Status200OK,
                   code: ResponseCodeConstants.SUCCESS,
                   message: "Thêm loại phòng thành công",
                   data: roomType
               ));
        }
        // Tạo RoomType mới
        [HttpPost]
        // [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ADMIN)]
        public async Task<IActionResult> CreateRoomType([FromBody] PortRoomTypeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomType = await _roomTypeService.CreateRoomType(model);
            return CreatedAtAction(nameof(GetAllRoomType), new { id = roomType.Id }, roomType);
        }

        // Xóa RoomType theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(string id)
        {
            await _roomTypeService.DeleteRoomType(id);

            return Ok(new BaseResponseModel<string ?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,  
                message: "Xóa loại phòng thành công"
            ));
        }

    }
}