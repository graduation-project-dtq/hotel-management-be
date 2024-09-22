using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public virtual ICollection<ServiceBooking> ? ServiceBookings { get; set; }
    }
}
