
using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class ImageFacilities 
    {
        [ForeignKey("Facilities")]
        public string FacilitiesID { get; set; }

        [ForeignKey("Image")]
        public string ImageID { get; set; }
        public virtual Facilities? Facilities { get; set; }
        public virtual Image? Image { get; set; }
    }
}
