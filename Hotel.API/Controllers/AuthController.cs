using Google.Apis.Auth.OAuth2.Requests;
using Hotel.Application.DTOs.UserDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="loginRequestDto"></param>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            LoginResponseDto loginResponseDto = await _authService.LoginAsync(loginRequestDto);
            return Ok(new BaseResponse<LoginResponseDto>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: loginResponseDto));
        }

        /// <summary>
        /// Đăng ký tài khoản khách hàng
        /// </summary>
        /// <param name="registerRequestDto"></param>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            await _authService.RegisterAsync(registerRequestDto);
            return Ok(new BaseResponse<string>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: "Đăng ký thành công hãy kiểm tra Email để xác thực tài khoản"));
        }

        /// <summary>
        /// Tạo token mới
        /// </summary>
        /// <param name="refeshTokenRequest"></param>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefeshTokenRequestDto refeshTokenRequest)
        {
            TokenResponseDto tokenResponseDto = await _authService.RefreshAccessTokenAsync(refeshTokenRequest);
            return Ok(new BaseResponse<TokenResponseDto>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: tokenResponseDto));
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> ActiveAccountAsync(string email, string code)
        {
              await _authService.ActiveAccountAsync(email,code);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Kích hoạt tài khoản thành công!"
                ));
        }
        [HttpPost("reponse-code")]
        public  async Task<IActionResult> ReponseCode(string email)
        {
            await _authService.ReponseCode(email);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Đã gửi mã kích hoạt tài khoản thành công!"
                ));
        }
    }
}
