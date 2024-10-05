using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class HouseType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
