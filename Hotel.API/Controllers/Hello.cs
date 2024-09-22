using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Hello : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult>Hi()
        {
            return Ok("Hello");
        }
    }
}
