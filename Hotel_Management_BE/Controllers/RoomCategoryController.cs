using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.ModelViews.RoomCategoryModelView;
using Hotel.Services.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Hotel.Core.Base.BaseException;

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
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] RoomCategoryModelView roomCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roomCategory = _mapper.Map<RoomCategory>(roomCategoryModel);
                await _roomCategoryService.Add(roomCategory);

                // Trả về mã trạng thái 201 Created cùng với thông tin về mục mới được tạo
                return CreatedAtAction(nameof(GetAllRoomCategoryActivate), new { id = roomCategory.Id }, roomCategory);
            }
            catch (DuplicateInternalCodeException ex)
            {
                _logger.LogWarning(ex, "Room category with the same InternalCode already exists.");
                return Conflict(new { message = ex.Message }); // Trả về mã lỗi 409 Conflict
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddRoomCategory");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
