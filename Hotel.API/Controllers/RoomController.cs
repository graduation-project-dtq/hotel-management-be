using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Application.Services;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
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


    }
}
