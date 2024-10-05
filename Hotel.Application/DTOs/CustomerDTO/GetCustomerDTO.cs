
namespace Hotel.Application.DTOs.CustomerDTO
{
    public class GetCustomerDTO
    {
        public string Id {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? IdentityCard { get; set; }
        public string? Sex { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
        public int? CredibilityScore { get; set; } //Điểm uy tín
        public int? AccumulatedPoints { get; set; } //Điểm tích luỹ
    }
}
