
using Hotel.Application.DTOs.UserDTO;
using Hotel.Domain.Entities;

namespace Hotel.Application.Interfaces
{
    public interface ITokenService
    {
        TokenResponseDto GenerateToken(Account account, string role);
        Task<TokenResponseDto> RefreshAccessToken(RefeshTokenRequestDto refeshTokenRequest);
    }
}
