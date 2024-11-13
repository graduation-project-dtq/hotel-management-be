using System;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.EmployeeDTO
{
    public class CreateEmployeeDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên!")]
        [StringLength(50, ErrorMessage = "Tên không được vượt quá 50 kí tự.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        [Required(ErrorMessage = "Vui lòng nhập email!")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 kí tự.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ!")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Số điện thoại phải có 10 kí tự.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số CMND/CCCD!")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "Số CMND/CCCD phải từ 9 đến 12 kí tự.")]
        public string IdentityCard { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn giới tính!")]
        [RegularExpression("^(Nam|Nữ|Khác)$", ErrorMessage = "Giới tính chỉ có thể là 'Nam', 'Nữ' hoặc 'Khác'.")]
        public string Sex { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh!")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreateEmployeeDTO), nameof(ValidateDateOfBirth))]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
        [StringLength(200, ErrorMessage = "Địa chỉ không được vượt quá 200 kí tự.")]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Xác thực ngày sinh không được lớn hơn ngày hiện tại.
        /// </summary>
        public static ValidationResult? ValidateDateOfBirth(DateOnly dateOfBirth, ValidationContext context)
        {
            var today = DateOnly.FromDateTime(DateTime.Now); // Lấy ngày hiện tại dưới dạng DateOnly
            if (dateOfBirth > today)
            {
                return new ValidationResult("Ngày sinh không được lớn hơn ngày hiện tại.");
            }
            return ValidationResult.Success;
        }

    }
}
