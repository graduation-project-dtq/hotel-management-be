using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.Services.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomCategoryController : ControllerBase
    {
        private readonly IRoomCategoryService _roomCategoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomCategoryController> _logger;

        public RoomCategoryController(IRoomCategoryService roomCategoryService, IMapper mapper, ILogger<RoomCategoryController> logger)
        {
            _roomCategoryService = roomCategoryService;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRoomCategoryActivate()
        {
            try
            {
                _logger.LogInformation("GetAllRoomCategoryActivate method called");
                IList<RoomCategory> roomCategories = await _roomCategoryService.GetAll();
                return Ok(roomCategories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllRoomCategoryActivate");
                return StatusCode(500, "An error occurred while processing your request.");
            }
           
        }
    }
}
