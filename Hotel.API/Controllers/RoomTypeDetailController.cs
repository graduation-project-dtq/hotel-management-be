using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("/GetByID{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            GetRoomTypeDetailDTO result = await _roomTypeDetailService.GetById(id);
            return Ok(new BaseResponse<GetRoomTypeDetailDTO>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               message:"Lấy thông tin thành công",
               data: result));
        }

        [HttpPost]
        [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.CUSTOMER)]
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
        public async Task<IActionResult> GetByRoomTypeId(string roomTypeID)
        {
            List<GetRoomTypeDetailDTO> roomTypeDetails = await _roomTypeDetailService.GetByRoomTypeId(roomTypeID);

            return Ok(new BaseResponse<List<GetRoomTypeDetailDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: roomTypeDetails));
        }
    }
 }
