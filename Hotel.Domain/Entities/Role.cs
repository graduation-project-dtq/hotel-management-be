using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
