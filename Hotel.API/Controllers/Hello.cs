using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Hello : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult>Hi()
        {
            return Ok("Hello");
        }
    }
}
