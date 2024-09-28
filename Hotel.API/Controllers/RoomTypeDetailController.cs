using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
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
                data: "Get RoomType success "));
        }

        [HttpPost]
        // [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ADMIN)]
        public async Task<IActionResult> CreateRoomTypeDetail([FromBody] PortRoomTypeDetailDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomTypeDetail = await _roomTypeDetailService.CreateRoomTypeDetail(model);
            return CreatedAtAction(nameof(GetAllRoomTypeDetail), new { id = roomTypeDetail.Id }, roomTypeDetail);
        }

    }
}
