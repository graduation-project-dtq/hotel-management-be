using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services;
using Hotel.ModelViews.RoomModelView;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        public RoomController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            IList<Room> categories = await _roomService.GetAll();
            return Ok(categories);
        }
        [HttpPost]
        public async Task<IActionResult> AddRoom(RoomModelView room)
        {
            try
            {
                // Map từ RoomCreateModelView sang Room
                var roomEntity = _mapper.Map<Room>(room);

                await _roomService.Add(roomEntity);
                IList<Room> categories = await _roomService.GetAll();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
