using Hotel.Application.DTOs.EvaluationDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(IRoomTypeService roomTypeService, ILogger<RoomTypeController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Lấy danh sách phòng loại phòng 
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

        /// <summary>
        /// Tìm kiếm loại phòng theo ID
        /// <param name="id"></param>

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

      
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> CreateRoomType([FromForm] List<IFormFile>? images, [FromForm] PortRoomTypeDTO model)
        {
            GetRoomTypeDTO result = await _roomTypeService.CreateRoomType(images, model);
            return Ok(new BaseResponseModel<GetRoomTypeDTO>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Tạo thành công"
            ));
        }
        /// <summary>
        /// Xoá loại phòng theo ID
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
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
