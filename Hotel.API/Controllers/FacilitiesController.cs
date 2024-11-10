using Hotel.Application.DTOs.FacilitiesDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Interfaces;
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
    }
}
