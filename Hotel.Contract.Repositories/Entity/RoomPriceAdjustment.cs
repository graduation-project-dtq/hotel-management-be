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
    [Table("RoomPriceAdjustment")]
    public class RoomPriceAdjustment : BaseEntity
    {
        [StringLength(50)]
        public string RoomPriceId { get; set; }
        [StringLength(50)]
        public string PriceAdjustmentPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual RoomPrice RoomPrice { get; set; }
        public virtual PriceAdjustmentPlan PriceAdjustmentPlan { get; set; }

    }
}
