namespace Hotel.Application.DTOs.VoucherDTO
{
    public class GetVoucherDTO
    {
        public string Id {  get; set; }=string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ? Description { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public string? CustomerId { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}
