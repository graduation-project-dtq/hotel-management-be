using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class ImageFacilities 
    {
        [ForeignKey("Facilities")]
        public string FacilitiesID { get; set; } = string.Empty;

        [ForeignKey("Image")]
        public string ImageID { get; set; } = string.Empty;
        public virtual Facilities? Facilities { get; set; }
        public virtual Image? Image { get; set; }
    }
}
