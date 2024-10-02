using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Domain.Entities
{
    public class ImageService 
    {
        [ForeignKey("Service")]
        public string ServiceID {  get; set; } = string.Empty;

        [ForeignKey("Image")]
        public string ImageID { get; set; } = string.Empty;

        public virtual Image ? Image { get; set; }
        public virtual Service ? Service { get; set; }
    }
}
