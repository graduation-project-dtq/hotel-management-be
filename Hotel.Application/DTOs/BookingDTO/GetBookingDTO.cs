using Hotel.Application.DTOs.BookingDetailDTO;
using Hotel.Application.DTOs.ServiceDTO;
using Hotel.Domain.Entities;


namespace Hotel.Application.DTOs.BookingDTO
{
    public class GetBookingDTO
    {
        public string ? Id {  get; set; }
        public string ? EmployeeId { get; set; }
        public string ?CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber {  get; set; }
     
        public decimal? Deposit { get; set; }
        //Tiền khuyến mãi
        public decimal? PromotionalPrice { get; set; }

        //Tổng tiền
        public decimal TotalAmount { get; set; }
        //Tiền chưa thanh toán
        public decimal? UnpaidAmount { get; set; }
        public string? BookingDate { get; set; }
        public string? CheckInDate { get; set; }
        public string? CheckOutDate { get; set; }
        public virtual ICollection<GetBookingDetailDTO> ? BookingDetail { get; set; }
        public virtual ICollection<GetServiceBookingDTO>? Services {  get; set; }
        public virtual ICollection<GetPunishesDTO> Punishes { get; set; }
    }
}
