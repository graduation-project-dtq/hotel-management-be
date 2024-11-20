
namespace Hotel.Application.DTOs.VoucherDTO
{
    public class PutVoucherDTO
    {
        public string? CustomerId { get; set; }
        public string ? Description { get; set; } 
        public decimal ?DiscountAmount { get; set; }
        public DateOnly ? StartDate { get; set; }
        public DateOnly ? EndDate { get; set; }
        public int ? Quantity { get; set; }
    }
}
