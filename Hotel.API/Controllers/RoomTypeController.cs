using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
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

        /// <summary>
        /// Tạo loại phòng mới
        /// <param name="id"></param>
        [HttpPost]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> CreateRoomType([FromBody] PortRoomTypeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomType = await _roomTypeService.CreateRoomType(model);
            return CreatedAtAction(nameof(GetAllRoomType), new { id = roomType.Id }, roomType);
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
