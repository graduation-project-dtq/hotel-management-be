using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Constants;
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

        /// <summary>
        /// Lấy danh sách hình ảnh
        /// </summary>
        /// <param name="id"></param>
        /// 
        [HttpGet]
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
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
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        public async Task<IActionResult> CreateImageAsync(PostImageDTO model)
        {
            var image = await _imageService.CreateImage(model);
            return Ok(new BaseResponseModel<GetImageDTO>(
                  statusCode: StatusCodes.Status200OK,
                  code: "Thêm ảnh thành công",
                  data: image
              ));
        }

        [HttpPost("image")]
        public async Task<IActionResult> Post(PostImageViewModel model)
        {
            await _imageService.Post(model);
            return Ok(new BaseResponseModel<string?>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    message: "Tạo mới hình ảnh thành công"));
        }
    }
}
