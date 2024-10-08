namespace Hotel.Application.DTOs.VoucherDTO
{
    public class GetVoucherDTO
    {
        public string Id {  get; set; }=string.Empty;
  
        public string Description { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
    }
}
