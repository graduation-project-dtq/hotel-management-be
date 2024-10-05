using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Domain.Entities
{
    public class Account : BaseEntity
    {
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string Password { get; set; }
        public string? Name { get; set; }

        [ForeignKey("Role")]
        public string RoleId { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }

    }
}
