using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.ModelViews.AccountModelView;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAccountService _accountService;

        private readonly ILogger<AuthenticateController> _logger;
        public AuthenticateController(IAccountService accountService,ILogger<AuthenticateController> logger)
        {
            _accountService = accountService;
            _logger= logger;
        }
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModelView model)
        {
            var token = await _accountService.SignIn(model);

            if (token != null)
            {
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid username or password");
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModelView model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid request payload" });
            }

            // Kiểm tra xem email có hợp lệ không
            var emailValidationResult = new EmailAddressAttribute().IsValid(model.Email);
            if (!emailValidationResult)
            {
                return BadRequest(new { code = "InvalidEmail", description = "Email is invalid." });
            }

            var result = await _accountService.SignUp(model);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }


    }
}
