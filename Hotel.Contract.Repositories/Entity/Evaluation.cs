using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("Evaluation")]
    public class Evaluation : BaseEntity
    {
        [StringLength(50)]
        public string CustomerId { get; set; }
        [StringLength(50)]
        public string RoomTypeDetailId { get; set; }
        public string Comment { get; set; }
        public int Starts { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual RoomTypeDetail RoomTypeDetail { get; set; }

    }
}
