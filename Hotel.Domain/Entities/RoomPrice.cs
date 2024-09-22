using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class RoomPrice : BaseEntity
    {
        public decimal BasePrice { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public virtual RoomTypeDetail ? RoomTypeDetail { get; set; }
        public virtual ICollection<RoomPriceAdjustment> ? RoomPriceAdjustments { get; set; }
    }
}
