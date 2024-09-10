using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.ModelViews.RoomTypeDetailsMovelView;
using Hotel.Services.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
