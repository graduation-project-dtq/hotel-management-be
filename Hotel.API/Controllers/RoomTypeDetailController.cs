using Hotel.Application.Interfaces;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeDetailController : ControllerBase
    {
        private readonly IRoomTypeDetailService _roomTypeDetailService;
        private readonly ILogger<RoomTypeDetailController> _logger;

        public RoomTypeDetailController(IRoomTypeDetailService roomTypeDetailService, ILogger<RoomTypeDetailController> logger)
        {
            _roomTypeDetailService = roomTypeDetailService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoomTypeDetail()
        {
            await _roomTypeDetailService.GetAllRoomTypeDetail();
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Register success "));
        }
    }
}
