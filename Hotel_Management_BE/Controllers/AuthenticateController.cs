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
        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInModelView signInViewModel)
        {
            var result = await _accountService.SignInAsync(signInViewModel);
            if (string.IsNullOrEmpty(result))
            {
                return Unauthorized();
            }
            return Ok(result);
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpModelView signUpViewModel)
        {
            var result = await _accountService.SignUpAsync(signUpViewModel);
            if (result.Succeeded)
            {
                return Ok(result.Succeeded);
            }
            return BadRequest(result.Errors);
        }


    }
}
