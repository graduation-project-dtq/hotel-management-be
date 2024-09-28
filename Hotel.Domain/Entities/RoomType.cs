using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RoomTypeDetail> ? RoomTypeDetails { get; set; }
        public virtual ICollection<ImageRoomType> ? ImageRoomTypes { get; set; }
    }
}
