using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Domain.Entities
{
    public class Role : BaseEntity
    {

        [Required]
        [StringLength(255)]
        public string RoleName { get; set; } = string.Empty;
        public virtual ICollection<Account> ? Accounts { get; set; } = new List<Account>();
    }
}
