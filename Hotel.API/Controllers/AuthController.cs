using Google.Apis.Auth.OAuth2.Requests;
using Hotel.Application.DTOs.UserDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
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

        /// <summary>
        /// Đăng nhập bằng Google
        /// </summary>
        /// <param name="googleSignInDto">Thông tin đăng nhập từ Google</param>
        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleSignInDto googleSignInDto)
        {
            try
            {
                if (googleSignInDto == null || string.IsNullOrEmpty(googleSignInDto.Email))
                {
                    return BadRequest(new BaseResponse<string>(
                        statusCode: StatusCodes.Status400BadRequest,
                        code: ResponseCodeConstants.FAILED,
                        message: "Thông tin đăng nhập không hợp lệ"));
                }

                LoginResponseDto loginResponseDto = await _authService.SignInWithGoogleAsync(googleSignInDto);

                return Ok(new BaseResponse<LoginResponseDto>(
                    statusCode: StatusCodes.Status200OK,
                    code: ResponseCodeConstants.SUCCESS,
                    data: loginResponseDto,
                    message: "Đăng nhập bằng Google thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse<string>(
                    statusCode: StatusCodes.Status500InternalServerError,
                    code: ResponseCodeConstants.FAILED,
                    message: "Đã xảy ra lỗi khi đăng nhập bằng Google: " + ex.Message));
            }
        }

     
    }
}
