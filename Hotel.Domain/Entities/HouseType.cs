using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class HouseType : BaseEntity
    {
        public string Name { get; set; }=string.Empty;
        public string ? Description { get; set; } = string.Empty;
        public virtual ICollection<Room> ?  Rooms { get; set; }
    }
}
