namespace Hotel.Application.DTOs.EmployeeDTO
{
    public class PutEmployeeDTO
    {
        public string? Name { get; set; } 
        public string? IdentityCard { get; set; } 
        public string ?Sex { get; set; } 
        public DateOnly? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; } 
    }
}
