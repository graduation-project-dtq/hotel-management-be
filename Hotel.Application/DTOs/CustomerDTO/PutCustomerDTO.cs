using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.CustomerDTO
{
    public class PutCustomerDTO
    {
        public string Name { get; set; } = string.Empty;
        
        public string  IdentityCard { get; set; } = string.Empty ;
        public string  Sex { get; set; } =string.Empty;
        public DateOnly DateOfBirth { get; set; }

        [Phone(ErrorMessage="Số điện thoại không hợp lệ")]
        public string Phone { get; set; }=string.Empty;
        public string? Address { get; set; }
    }
}
