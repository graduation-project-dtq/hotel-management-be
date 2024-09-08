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

    [Table("PriceAdjustmentPlan")]
    public class PriceAdjustmentPlan : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PriceAdjustmentPlan()
        {
            RoomPriceAdjustments = new HashSet<RoomPriceAdjustment>();
        }
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string AdjustmentType { get; set; }

        public decimal? AdjustmentValue { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomPriceAdjustment> RoomPriceAdjustments { get; set; }
}
}
