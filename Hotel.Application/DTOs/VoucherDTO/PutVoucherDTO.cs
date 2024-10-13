using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.VoucherDTO
{
    public class PutVoucherDTO
    {
        public string? CustomerId { get; set; }
        public string ? Description { get; set; } 

        [Required(ErrorMessage ="Vui lòng nhập số tiền giảm giá")]
        public decimal DiscountAmount { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày bắt đầu")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập ngày giảm giá")]
        public DateOnly EndDate { get; set; }
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        public string Code { get; set; } = string.Empty;
    }
}
