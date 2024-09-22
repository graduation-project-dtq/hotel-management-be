
namespace Hotel.Application.DTOs
{
    public class RegisterRequestDto
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public required string Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? BranchId { get; set; }
    }
}
