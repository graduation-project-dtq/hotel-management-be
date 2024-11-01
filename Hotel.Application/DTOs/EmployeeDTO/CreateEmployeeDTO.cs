using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.EmployeeDTO
{
    public class CreateEmployeeDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        [EmailAddress(ErrorMessage = "Email is not valid")]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Phone number is not valid")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Phone number must be 10 digits")]
        public string? NumberPhone { get; set; }
        public string? InternalCode { get; set; }
        public string? AccountId { get; set; }
     
    }
}
