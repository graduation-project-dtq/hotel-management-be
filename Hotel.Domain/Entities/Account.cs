using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [ForeignKey("Role")]
        public string ? Name {  get; set; }
        public string RoleId { get; set; }
        public bool IsActive { get; set; }
        
    }
}
