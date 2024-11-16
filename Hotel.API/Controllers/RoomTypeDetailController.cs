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

        /// <summary>
        /// Lấy danh sách loại phòng
        /// <param name="id"></param>

        [HttpGet]
        public async Task<IActionResult> GetAllRoomTypeDetail()
        {
            var roomTypeDetails = await _roomTypeDetailService.GetAllRoomTypeDetail();
            return Ok(new BaseResponse<List<GetRoomTypeDetailDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: roomTypeDetails));
        }
       
        
        /// <summary>
        /// Tạo loại phòng mới
        /// <param name="model"></param>
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> CreateRoomTypeDetail([FromForm] List<IFormFile>? images,[FromForm] PostRoomTypeDetailDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            GetRoomTypeDetailDTO result = await _roomTypeDetailService.CreateRoomTypeDetail(images,model);
            return Ok(new BaseResponse<GetRoomTypeDetailDTO>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: result,
             message:"Tạo thành công"
             ));
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> UpdateRoomTypeDetail([FromForm] string id,[FromForm] List<IFormFile>? images, [FromForm] PutRoomTypeDetailDTO model)
        {

            GetRoomTypeDetailDTO result = await _roomTypeDetailService.UpdateRoomTypeDetail(id,images, model);
            return Ok(new BaseResponse<GetRoomTypeDetailDTO>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: result,
             message: "Cập nhật thành công"
             ));
        }


        [HttpDelete]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> DeleteRoomTypeDetail(string id)
        {

            await _roomTypeDetailService.DeleteRoomTypeDetailAsync(id);
            return Ok(new BaseResponse<string?>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: null,
             message: "Xoá thành công"
             ));
        }

        /// <summary>
        /// Tìm kiếm loại phòng theo ID
        /// <param name="roomtypedetailId"></param>

        [HttpGet("/GetByID{roomtypedetailId}")]
        public async Task<IActionResult> GetById(string roomtypedetailId)
        {
            GetRoomTypeDetailDTO result = await _roomTypeDetailService.GetById(roomtypedetailId);
            return Ok(new BaseResponse<GetRoomTypeDetailDTO>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               message:"Lấy thông tin thành công",
               data: result));
        }

       


        /// <summary>
        /// Gợi ý phòng 
        /// <param name="id"></param>
        [HttpGet("{soNguoi},{roomTypeDetailID}")]
        public async Task<IActionResult> FindRoom(int soNguoi, string roomTypeDetailID)
        {
            List<List<GetRoomTypeDetailDTO>> roomTypeDetails = await _roomTypeDetailService.FindRoom(soNguoi, roomTypeDetailID);

            return Ok(new BaseResponse<List<List<GetRoomTypeDetailDTO>>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: roomTypeDetails));
        }

        /// <summary>
        /// Tìm kiếm loại phòng theo ID loại bự
        /// <param name="id"></param>

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
