using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace XuongMay_BE.Contract.Repositories.Entities
{
    public class User : IdentityUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            //Orders = new HashSet<Order>();
            //ProductionLines = new HashSet<ProductionLine>();
        }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string Image { get; set; }

        [Key]
        [StringLength(450)]
        public override string Id { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset DeletedAt { get; set; }


    }
}
