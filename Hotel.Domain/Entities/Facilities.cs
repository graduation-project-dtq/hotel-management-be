

using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class Facilities : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<FacilitiesRoom> ? FacilitiesRooms { get; set; }
        public virtual ICollection<Punish> ? Punishes { get; set; }
        public virtual ICollection<ImageFacilities>? ImageFacilities { get; set; }
    }
}
