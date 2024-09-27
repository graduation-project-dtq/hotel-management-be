using Hotel.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index, int pageSize, string idSearch)
        {
            return null;
        }
    }
}
