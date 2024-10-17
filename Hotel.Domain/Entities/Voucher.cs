using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;


namespace Hotel.Domain.Entities
{
    public class Voucher : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        [ForeignKey("Customer")]
        public string ? CustomerId {  get; set; }
        public string ? Description { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public string Code { get; set; } = string.Empty;
        public DateTime? UsedDate { get; set; }
        public virtual ICollection<Booking>? Bookings { get; set; }
        public virtual Customer ? Customer { get; set; }
        public virtual ICollection<VoucherRoomTypeDetail>? VoucherRoomTypeDetails { get; set; }
    }
}
