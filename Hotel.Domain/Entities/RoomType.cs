using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string ?Description { get; set; } 
        public float AverageStart { get; set; }
        public virtual ICollection<RoomTypeDetail> ? RoomTypeDetails { get; set; }
        public virtual ICollection<ImageRoomType> ? ImageRoomTypes { get; set; }
        public virtual ICollection<Evaluation> ? Evaluations { get; set; }
        public RoomType() 
        {
            AverageStart = 5;
        }
    }
}
