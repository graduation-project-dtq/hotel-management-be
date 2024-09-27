using System.ComponentModel.DataAnnotations.Schema;

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
