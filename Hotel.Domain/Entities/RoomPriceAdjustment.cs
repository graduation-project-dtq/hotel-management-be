using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class RoomPriceAdjustment 
    {
        [ForeignKey("RoomTypeDetail")]
        public string RoomTypeDetailId { get; set; } = string.Empty;

        [ForeignKey("PriceAdjustmentPlan")]
        public string PriceAdjustmentPlanId { get; set; } = string.Empty;

        public virtual RoomTypeDetail ? RoomTypeDetail { get; set; }
        public virtual PriceAdjustmentPlan ? PriceAdjustmentPlan { get; set; }
    }
}
