using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public virtual ICollection<RoomTypeDetail> ? RoomTypeDetails { get; set; }
        public virtual ICollection<ImageRoomType> ? ImageRoomTypes { get; set; }
    }
}
