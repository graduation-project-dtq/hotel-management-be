using Hotel.Core.Base;


namespace Hotel.Domain.Entities
{
    public class Voucher : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public int MinCreditScoreRequired { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<CustomerVoucher> ? CustomerVouchers { get; set; }
        public virtual ICollection<VoucherRoomTypeDetail>? VoucherRoomTypeDetails { get; set; }
    }
}
