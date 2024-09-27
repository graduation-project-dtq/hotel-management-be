using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly ILogger<RoomTypeController> _logger;

        public RoomTypeController(IRoomTypeService roomTypeService, ILogger<RoomTypeController> logger)
        {
            _roomTypeService = roomTypeService;
            _logger = logger;
        }
        // Lấy tất cả RoomType
        [HttpGet]
        public async Task<IActionResult> GetAllRoomType()
        {
            var roomTypes = await _roomTypeService.GetAllRoomType();
            return Ok(roomTypes);
        }
        [HttpGet("All")]
        public async Task<IActionResult> Get()
        { 
            var roomTypes = await _roomTypeService.Get();
            return Ok(roomTypes);
        }
        // Tạo RoomType mới
        [HttpPost]
        public async Task<IActionResult> CreateRoomType([FromBody] CreateRoomTypeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var roomType = await _roomTypeService.CreateRoomType(model);
            return CreatedAtAction(nameof(GetAllRoomType), new { id = roomType.Id }, roomType);
        }

        // Xóa RoomType theo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomType(string id)
        {
            try
            {
                await _roomTypeService.DeleteRoomType(id);
                return NoContent();
            }
            catch (ErrorException ex)
            {
                _logger.LogError(ex, $"Delete RoomType failed for ID {id}");
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        
    }
}
