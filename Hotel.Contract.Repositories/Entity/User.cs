using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Contract.Repositories.Entity
{
    public class User : IdentityUser
    {
        public User()
        {
            CreatedTime = LastUpdatedTime = DateTimeOffset.Now; // Sử dụng DateTimeOffset.Now để lấy thời gian hiện tại với múi giờ
        }
   

        [Required]
        [StringLength(255)]
        public string? FullName { get; set; }

        public bool IsActive { get; set; }

        public string? CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        public string? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
    }
}
