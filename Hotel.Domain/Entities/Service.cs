using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class Service : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ? Description { get; set; } = string.Empty;
        public virtual ICollection<ServiceBooking> ? ServiceBookings { get; set; }
        public virtual ICollection<ImageService> ? ImageServices { get; set; }
    }
}
