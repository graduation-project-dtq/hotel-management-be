using Hotel.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace Hotel.Contract.Repositories.Entity
{
    public class User : IdentityUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
        }

        [Required]
        public string Fullname { get; set; }

        [Required]
        public string ? Image { get; set; }

        [Key]
        [StringLength(50)]
        public override string Id { get; set; }


        [StringLength(50)]
        public string? EmployeeId { get; set; }

        [StringLength(50)]
        public string? CustomerId { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? DeletedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset DeletedAt { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
