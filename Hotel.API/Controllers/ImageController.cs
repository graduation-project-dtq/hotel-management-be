using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Domain.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }
        [HttpGet]
        [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.CUSTOMER)]
        public async Task<IActionResult> GetAllImageAsync()
        {
           var images=await _imageService.GetAllImage();
            return Ok(new BaseResponseModel<List<GetImageDTO>>(
                  statusCode: StatusCodes.Status200OK,
                  code: "Lấy danh sách ảnh thành công",
                  data: images
              ));
        }

        [HttpPost]
        [Authorize(Roles = CLAIMS_VALUES.ROLE_TYPE.CUSTOMER)]
        public async Task<IActionResult> CreateImageAsync(PostImageDTO model)
        {
            var image = await _imageService.CreateImage(model);
            return Ok(new BaseResponseModel<GetImageDTO>(
                  statusCode: StatusCodes.Status200OK,
                  code: "Thêm ảnh thành công",
                  data: image
              ));
        }
    }
}
