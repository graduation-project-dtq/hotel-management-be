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

        [HttpGet]
        public async Task<IActionResult> GetAllFloor()
        {
          List<GetFloorDTO> floors=  await _floorService.GetAllFloor();
            return Ok(new BaseResponse<List<GetFloorDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: floors));
        }

        [HttpPost]
        public async Task<IActionResult> CreateFloor([FromBody] PostFloorDTO model)
        {
            GetFloorDTO floor =await _floorService.CreateFloor(model);
            return Ok(new BaseResponse<GetFloorDTO>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: floor));
        }


    }
}
