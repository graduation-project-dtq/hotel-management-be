using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.CustomerDTO
{
    public class CreateCustomerDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty; //Thêm khi booking off
        public string ? IdentityCard { get; set; } = string.Empty; //Thêm khi booking off
        public string ? Sex {  get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; } //Thêm khi booking off
        public string? Email { get; set; }
        public string? Phone { get; set; } //Thêm khi booking off
        public string? AccountId { get; set; }
        public string? Address { get; set; }

    }
}
