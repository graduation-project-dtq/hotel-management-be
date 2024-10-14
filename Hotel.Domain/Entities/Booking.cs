using Hotel.Core.Base;
using Hotel.Domain.Enums.EnumBooking;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Booking : BaseEntity
    {
        [ForeignKey("Employee")]
        public string ? EmployeeId { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        [ForeignKey("Voucher")]
        public string ? VoucherId { get; set; } = string.Empty;
        public string PhoneNumber {  get; set; } = string.Empty;
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        //Tiền đặt cọc
        public decimal  Deposit {  get; set; }
        //Tiền khuyến mãi
        public decimal  PromotionalPrice { get; set; }
        //Tổng tiền
        public decimal TotalAmount { get; set; }
        //Tiền chưa thanh toán
        public decimal  UnpaidAmount {  get; set; }
        public EnumBooking Status { get; set; }
        public string ? CustomerName {  get; set; }
        public string ? IdentityCard { get; set; }
        public virtual Customer ? Customer { get; set; }
        public virtual Employee ? Employee { get; set; }
        public virtual Voucher ? Voucher { get; set; }
        public virtual ICollection<BookingDetail> ? BookingDetails { get; set; }
        public virtual ICollection<ServiceBooking> ? ServiceBookings { get; set; }
        public virtual ICollection<Punish>  ? Punishes { get; set; }
    }
}
