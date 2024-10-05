
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.UserDTO
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [PasswordPropertyText]
        public string Password { get; set; } = string.Empty;
    }

}
