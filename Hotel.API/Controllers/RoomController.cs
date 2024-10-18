using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
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

        /// <summary>
        /// Lấy danh sách phòng
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch"></param>
        /// <param name="nameSearch"></param>
        [HttpGet]
        //[Authorize(Roles = "ADMIN,EMPLOYEE")]
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
        /// <summary>
        /// Tìm phòng bằng ID
        /// <param name="id"></param>
        //Tìm theo id
        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult>GetRoomById(string id)
        {
            var room = await _roomService.GetRoomById(id);
            return Ok(new BaseResponseModel<GetRoomDTO>(
                   statusCode: StatusCodes.Status200OK,
                   code: "Lấy thông tin phòng thành công",
                   data: room
               ));
        }


        /// <summary>
        /// Tạo phòng mới
        /// <param name="model"></param>

        [HttpPost]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
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
        /// <summary>
        /// Lấy danh sách phòng
        [HttpPost("FindRoom")]
        public async Task<IActionResult> FindRoomBooking([FromBody] FindRoomDTO findRoomDTO)
        {
            // Tìm kiếm phòng
            List<GetRoomDTO> rooms = await _roomService.FindRoomBooking(findRoomDTO);
            if(rooms == null)
            {
                return Ok(new BaseResponseModel<string ?>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: null,
              message: "Không còn phòng nào trống"
          ));
            }    
            // Số lượng phòng còn trống
            int availableRoomCount = rooms.Count;

            return Ok(new BaseResponseModel<object>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: new { AvailableRooms = availableRoomCount, RoomList = rooms },
                message: "Tìm phòng thành công"
            ));
        }
    }
}
