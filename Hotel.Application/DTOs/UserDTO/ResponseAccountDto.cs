namespace Hotel.Application.DTOs.UserDTO;
    
public class ResponseAccountDto
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTimeOffset CreatedTime { get; set; }
}