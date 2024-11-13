namespace Hotel.Application.DTOs.EmployeeDTO
{
    public class GetEmployeeDTO
    {
        public string Id { get; set; }= string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateOnly HireDate { get; set; }
    }
}
