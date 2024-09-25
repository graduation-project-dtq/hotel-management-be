using Hotel.Application.DTOs.FloorDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.Services;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private IFloorService _floorService;
        private ILogger<FloorController> _logger;
        public FloorController(IFloorService floorService, ILogger<FloorController> logger)
        {
            _floorService = floorService;
            _logger = logger;
        }
        //[HttpGet]
        //public async Task<List<GetFloorDTO>> GetAllFloor()
        //{
        //    await _floorService.GetAllFloor();
        //    return Ok(new BaseResponse<string>(
        //        statusCode: StatusCodes.Status200OK,
        //        code: ResponseCodeConstants.SUCCESS,
        //        data: "Get RoomType success "));
        //}
    }
}
