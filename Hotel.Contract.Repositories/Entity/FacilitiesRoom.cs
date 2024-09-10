using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("FacilitiesRoom")]
    public class FacilitiesRoom
    {
        [StringLength(50)]
        public string RoomID { get; set; }

        [StringLength(50)]
        public string FacilitiesID { get; set; }
        public int Quantity { get; set; }

        public virtual Room Room { get; set; }
        public virtual Facilities Facilities { get; set; }

    }
}
