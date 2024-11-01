using Hotel.Application.DTOs.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<TokenResponseDto> RefreshAccessTokenAsync(RefeshTokenRequestDto refeshTokenRequest);
        Task ActiveAccountAsync(string email, string code);
        Task ReponseCode(string email);
        Task<LoginResponseDto> SignInWithGoogleAsync(GoogleSignInDto googleSignInDto);
        Task DeleteAccount(string id);
    }
}
