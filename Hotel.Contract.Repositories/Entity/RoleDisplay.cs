using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("RoleDisplay")]
    public class RoleDisplay
    {
        
        public string RoleId { get; set; }
  
        public string DisplayID { get; set; }

        public virtual Role Role { get; set; }
        public virtual Display Display { get; set; }

    }
}
