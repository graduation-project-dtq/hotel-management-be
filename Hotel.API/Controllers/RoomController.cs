using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Hotel.Domain.Entities;
using Hotel.Infrastructure.IOW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;
        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index=1, int pageSize=10, string idSearch="", string nameSearch="")
        {
            PaginatedList<GetRoomDTO> result = await _roomService.GetPageAsync(index, pageSize, idSearch, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetRoomDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                message: "Lấy danh sách phòng thành công!",
                data: result
            ));
        }
        //[HttpGet]
        //public async Task<IActionResult> GetAllRoom()
        //{
        //    return Ok(await _roomService.GetAllRoom());
        //}

        //Tìm theo id
        [HttpGet("{id}")]
        public async Task<IActionResult>GetRoomById(string id)
        {
            var room = await _roomService.GetRoomById(id);
            return Ok(new BaseResponseModel<GetRoomDTO>(
                   statusCode: StatusCodes.Status200OK,
                   code: "Lấy thông tin phòng thành công",
                   data: room
               ));
        }
        
        [HttpPost]
        [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.CUSTOMER)]
        public async Task<IActionResult>CreateRoom([FromBody] PostRoomDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var room = await _roomService.CreateRoom(model);
            return Ok(new BaseResponseModel<GetRoomDTO>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: room,
                    message:"Thêm phòng thành công"
                ));
        }

        [HttpPost("FindRoom")]
        public async Task<IActionResult> FindRoomBooking([FromBody] FindRoomDTO request)
        {
            // Kiểm tra đầu vào
            if (request.CheckInDate >= request.CheckOutDate)
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.BADREQUEST,
                    message: "Ngày check-in phải nhỏ hơn ngày check-out."
                ));
            }

            if (string.IsNullOrWhiteSpace(request.RoomTypeDetailID))
            {
                return BadRequest(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status400BadRequest,
                    code: ResponseCodeConstants.BADREQUEST,
                    message: "ID loại phòng không được để trống."
                ));
            }

            // Tìm kiếm phòng
            List<GetRoomDTO> result;
            try
            {
                result = await _roomService.FindRoomBooking(request.CheckInDate, request.CheckOutDate, request.RoomTypeDetailID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tìm phòng booking");

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status500InternalServerError,
                    code: ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    message: "Đã xảy ra lỗi trong quá trình tìm kiếm phòng."
                ));
            }

            // Kiểm tra kết quả
            if (result == null || !result.Any())
            {
                return NotFound(new BaseResponseModel<string>(
                    statusCode: StatusCodes.Status404NotFound,
                    code: ResponseCodeConstants.NOT_FOUND,
                    message: "Không tìm thấy phòng nào phù hợp với yêu cầu."
                ));
            }

            return Ok(new BaseResponseModel<List<GetRoomDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Tìm phòng thành công"
            ));
        }

    }
}
