namespace Hotel.Application.DTOs.UserDTO;
    
public class ResponseAccountDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTimeOffset CreatedTime { get; set; }
}