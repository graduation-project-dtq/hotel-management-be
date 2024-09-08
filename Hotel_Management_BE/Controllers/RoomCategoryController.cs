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
        public RoomCategoryController(IRoomCategoryService roomCategoryService)
        {
            _roomCategoryService = roomCategoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllRoomCategory()
        {
            IList<RoomCategory> roomCategories = await _roomCategoryService.GetAll();
            return Ok(roomCategories);
        }
    }
}
