using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class RoomPriceAdjustment 
    {
        [ForeignKey("RoomPrice")]
        public string RoomPriceId { get; set; }

        [ForeignKey("PriceAdjustmentPlan")]
        public string PriceAdjustmentPlanId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public virtual RoomPrice ? RoomPrice { get; set; }
        public virtual PriceAdjustmentPlan ? PriceAdjustmentPlan { get; set; }
    }
}
