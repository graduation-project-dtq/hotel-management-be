using Hotel.Application.DTOs.BookingDetailDTO;
using Hotel.Application.DTOs.ServiceDTO;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class PostBookingDTO
    {
        public string? EmployeeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã khách hàng!")]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngày nhận phòng!")]
        public DateOnly CheckInDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày trả phòng!")]
        [CustomValidation(typeof(PostBookingDTO), nameof(ValidateDates))]
        public DateOnly CheckOutDate { get; set; }

        public string? VoucherId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tiền đặt cọc phải lớn hơn hoặc bằng 0.")]
        public decimal Deposit { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có 10 ký tự.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public virtual ICollection<PostBookingDetailDTO>? BookingDetails { get; set; }
        public virtual ICollection<PostServiceBookingDTO>? Services { get; set; }

        /// <summary>
        /// Xác thực ngày nhận phòng phải nhỏ hơn ngày trả phòng.
        /// </summary>
        public static ValidationResult? ValidateDates(object? value, ValidationContext context)
        {
            var instance = context.ObjectInstance as PostBookingDTO;
            if (instance == null || instance.CheckInDate == default || instance.CheckOutDate == default)
            {
                return ValidationResult.Success;
            }

            if (instance.CheckInDate >= instance.CheckOutDate)
            {
                return new ValidationResult("Ngày trả phòng phải lớn hơn ngày nhận phòng.");
            }

            return ValidationResult.Success;
        }
    }
}
