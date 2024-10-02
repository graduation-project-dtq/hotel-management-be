using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Entities;
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
            var roomTypeDetails = await _roomTypeDetailService.GetAllRoomTypeDetail();
            return Ok(new BaseResponse<List<GetRoomTypeDetailDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: roomTypeDetails));
        }


        [HttpPost]
        // [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.ADMIN)]
        public async Task<IActionResult> CreateRoomTypeDetail([FromBody] PostRoomTypeDetailDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomTypeDetail = await _roomTypeDetailService.CreateRoomTypeDetail(model);
            return Ok(new BaseResponse<RoomTypeDetail>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: roomTypeDetail));
        }

        [HttpGet("{soNguoi},{roomTypeDetailID}")]
        public async Task<IActionResult> FindRoom(int soNguoi, string roomTypeDetailID)
        {
            List<List<GetRoomTypeDetailDTO>> roomTypeDetails = await _roomTypeDetailService.FindRoom(soNguoi, roomTypeDetailID);

            return Ok(new BaseResponse<List<List<GetRoomTypeDetailDTO>>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: roomTypeDetails));
        }

        [HttpGet("{roomTypeID}")]
        public async Task<IActionResult> GetByRoomTypeId(string id)
        {
            List<GetRoomTypeDetailDTO> roomTypeDetails = await _roomTypeDetailService.GetByRoomTypeId(id);

            return Ok(new BaseResponse<List<GetRoomTypeDetailDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: roomTypeDetails));
        }
    }
 }
