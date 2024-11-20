using Hotel.Application.DTOs.UserDTO;

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
        Task ChangePassWordAsync(string email,string password, string newPassWord, string reNewPassWord);
        Task LockAccount(string id);
    }
}
