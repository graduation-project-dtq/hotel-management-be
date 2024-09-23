namespace Hotel.Application.DTOs.UserDTO;
public class TokenResponseDto
{
    public TokenResponseDto() { }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required ResponseAccountDto Account { get; set; }
}
