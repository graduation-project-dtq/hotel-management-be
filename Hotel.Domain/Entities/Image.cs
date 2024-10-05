using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class Image : BaseEntity
    {
        public string URL {  get; set; } = string.Empty;

        public virtual ICollection<ImageEvaluation> ? Evaluations { get; set; }
        public virtual ICollection<ImageFacilities> ? ImageFacilities { get; set; }
        public virtual ICollection<ImageRoomType> ? ImageRoomTypes { get; set; }
        public virtual ICollection<ImageRoomTypeDetail> ? ImageRoomTypesDetail { get; set; }
        public virtual ICollection<ImageService> ? ImageServices { get; set; }
    }
}
