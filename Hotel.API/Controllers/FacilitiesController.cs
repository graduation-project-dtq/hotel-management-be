using Hotel.Application.DTOs.FacilitiesDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Application.Services;
using Hotel.Core.Constants;
using Hotel.Domain.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacilitiesController : ControllerBase
    {
        private readonly IFacilitiesService _facilitiesService;
        public FacilitiesController(IFacilitiesService facilitiesService)
        {
            _facilitiesService = facilitiesService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "",
        string nameSearch = "")
        {
            PaginatedList<GetFacilitiesDTO> result = await _facilitiesService.GetPageAsync(index, pageSize, idSearch, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetFacilitiesDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Lấy danh sách nội thất thành công"
            ));
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> CreateFacilities([FromForm]ICollection<IFormFile>? images, [FromForm] PostFacilitiesDTO model)
        {
            GetFacilitiesDTO result = await _facilitiesService.CreateFacilities(images, model);
            return Ok(new BaseResponseModel<GetFacilitiesDTO>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Tạo thành công"
            ));
        }

        [HttpPut]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]

        [HttpGet("room")]
        public async Task<IActionResult> GetFacilitiesByRoomId(int index = 1, int pageSize = 10, string roomId ="",
         string nameSearch="")
        {
            PaginatedList<GetFacilitiesRoomDTO> result = await _facilitiesService.GetFacilitiesByRoomId(index, pageSize, roomId, nameSearch);
            return Ok(new BaseResponseModel<PaginatedList<GetFacilitiesRoomDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Lấy danh sách nội thất thành công"
            ));
        }
    }
}
