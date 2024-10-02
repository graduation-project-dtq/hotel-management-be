
using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class ViewHotel : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public virtual ICollection<RoomView> ? RoomViews { get; set; }
    }
}
