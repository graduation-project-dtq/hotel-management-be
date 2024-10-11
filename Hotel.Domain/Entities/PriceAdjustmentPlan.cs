
using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class PriceAdjustmentPlan : BaseEntity
    {

        public string Name { get; set; } = string.Empty;
        public decimal AdjustmentValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ? Description { get; set; } = string.Empty;
        public virtual ICollection<RoomPriceAdjustment> ? RoomPriceAdjustments { get; set; }

    }
}
