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
       
        public string Name { get; set; }
        public string AdjustmentType { get; set; }
        public decimal AdjustmentValue { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RoomPriceAdjustment> RoomPriceAdjustments { get; set; }

    }
}
