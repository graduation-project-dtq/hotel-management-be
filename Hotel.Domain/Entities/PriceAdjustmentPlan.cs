
using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class PriceAdjustmentPlan : BaseEntity
    {

        public string Name { get; set; }
        public string AdjustmentType { get; set; }
        public decimal AdjustmentValue { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RoomPriceAdjustment> ? RoomPriceAdjustments { get; set; }

    }
}
