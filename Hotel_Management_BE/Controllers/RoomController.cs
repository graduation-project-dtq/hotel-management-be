using AutoMapper;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.Core.App;
using Hotel.ModelViews.RoomModelView;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = AppRole.DefaultRole + "," + AppRole.Administrator)]
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
        [Authorize(Roles = AppRole.DefaultRole + "," + AppRole.Administrator)]
        public async Task<IActionResult> GetAllRoom()
        {
            IList<Room> rooms = await _roomService.GetAll();
            return Ok(rooms);
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
