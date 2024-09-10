using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.Core.App;
using Hotel.ModelViews.RoomModelView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, IMapper mapper, ILogger<RoomController> logger)
        {
            _roomService = roomService ?? throw new ArgumentNullException(nameof(roomService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                _logger.LogInformation("GetAllRooms method called");
                IList<Room> rooms = await _roomService.GetAll();
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllRooms");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
     
        public async Task<IActionResult> AddRoom([FromBody] RoomModelView roomModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var roomEntity = _mapper.Map<Room>(roomModel);
                await _roomService.Add(roomEntity);
                return CreatedAtAction(nameof(GetAllRooms), new { id = roomEntity.Id }, roomEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddRoom");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // Thêm các action methods khác nếu cần
    }
}