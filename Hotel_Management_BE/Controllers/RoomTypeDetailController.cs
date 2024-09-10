using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.ModelViews.RoomTypeDetailsMovelView;
using Hotel.Services.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Hotel.Core.Base.BaseException;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeDetailController : ControllerBase
    {
        private readonly IRoomTypeDetailService _roomTypeDetailService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomTypeDetailController> _logger;

        public RoomTypeDetailController(IRoomTypeDetailService roomTypeDetailService, IMapper mapper, ILogger<RoomTypeDetailController> logger)
        {
            _roomTypeDetailService = roomTypeDetailService ?? throw new ArgumentNullException(nameof(roomTypeDetailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActivate()
        {
            try
            {
                _logger.LogInformation("GetAllRoomTypeDetail method called");
                IList<RoomTypeDetail> roomTypeDetails = await _roomTypeDetailService.GetAllActive();
                //Convert list RoomTypeDetail to RoomTypeDetailMovelView
                IList<RoomTypeDetailMovelView> roomTypeDetailModelViews = _mapper.Map<IList<RoomTypeDetailMovelView>>(roomTypeDetails);
                return Ok(roomTypeDetailModelViews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllRoomTypeDetail");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("GetAllRoomTypeDetail method called");
                IList<RoomTypeDetail> roomTypeDetails = await _roomTypeDetailService.GetAll();
                //Convert list RoomTypeDetail to RoomTypeDetailMovelView
                IList<RoomTypeDetailMovelView> roomTypeDetailModelViews = _mapper.Map<IList<RoomTypeDetailMovelView>>(roomTypeDetails);
                return Ok(roomTypeDetailModelViews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllRoomTypeDetail");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RoomTypeDetailMovelView roomTypeDetailMovel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roomTypeDetail = _mapper.Map<RoomTypeDetail>(roomTypeDetailMovel);
                await _roomTypeDetailService.Add(roomTypeDetail);

                // Trả về mã trạng thái 201 Created cùng với thông tin về mục mới được tạo
                return CreatedAtAction(nameof(GetAllActivate), new { id = roomTypeDetail.Id }, roomTypeDetail);
            }
            catch (DuplicateInternalCodeException ex)
            {
                _logger.LogWarning(ex, "RoomTypeDetail with the same InternalCode already exists.");
                return Conflict(new { message = ex.Message }); // Trả về mã lỗi 409 Conflict
            }
            catch (ForeignKeyViolationException ex)
            {
                _logger.LogWarning(ex, "Foreign key violation occurred.");
                return BadRequest(new { message = ex.Message }); // Trả về mã lỗi 400 Bad Request
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database update error occurred.");
                return StatusCode(500, "Database update error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddRoomTypeDetail");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
