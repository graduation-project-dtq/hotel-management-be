using Hotel.Contract.Services.IService;
using Hotel.ModelViews.AccountModelView;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthenticateController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInViewModel signInViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid data", Errors = ModelState });
            }

            var result = await _accountService.SignInAsync(signInViewModel);

            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            return Ok(new { Value = result });
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpViewModel signUpViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid data", Errors = ModelState });
            }

            var result = await _accountService.SignUpAsync(signUpViewModel);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully" });
            }

            return BadRequest(new { Message = "Registration failed", Errors = result.Errors.Select(e => e.Description) });
        }
    }
}
