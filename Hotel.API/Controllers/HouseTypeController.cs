using Hotel.Application.DTOs.HouseTypeDTO;
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HouseTypeController : ControllerBase
    {
        private readonly IHouseTypeService _houseTypeService;
        public HouseTypeController(IHouseTypeService houseTypeService)
        {
            _houseTypeService = houseTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPageAsync (int index = 1, int pageSize = 10, string idSearch = "", string nameSearch = "")
        {
            PaginatedList<GetHoustTypeDTO> result= await _houseTypeService.GetPageAsync(index, pageSize, idSearch, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetHoustTypeDTO>>(
                 statusCode: StatusCodes.Status200OK,
                 code: ResponseCodeConstants.SUCCESS,
                 message:"Lấy danh sách loại phòng thành công",
                 data: result
             ));
        }
        [HttpPost]
        public async Task<IActionResult> CreateHouseType([FromBody]PostHouseTypeDTO model)
        {
            await _houseTypeService.CreateHouseType(model);
            return Ok(new BaseResponseModel<string ?>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               message: "Tạo loại phòng thành công",
               data: null
           ));
        }
    }
}
