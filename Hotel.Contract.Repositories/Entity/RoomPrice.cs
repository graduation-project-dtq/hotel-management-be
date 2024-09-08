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
    [Table("RoomPrice")]
    public class RoomPrice : BaseEntity
    {
        public RoomPrice()
        {
            RoomPriceAdjustments = new HashSet<RoomPriceAdjustment>();
        }
        [StringLength(50)]
        public string RoomTypeDetailId { get; set; }

        public decimal BasePrice { get; set; }

        public int? Capacity { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public virtual RoomTypeDetail RoomTypeDetail { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomPriceAdjustment> RoomPriceAdjustments { get; set; }
    }
}
