using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
namespace Hotel.Domain.Entities
{
    public class ImageService : BaseEntity
    {
        [ForeignKey("Service")]
        public string ServiceID {  get; set; }

        [ForeignKey("Image")]
        public string ImageId { get; set; }

        public virtual Image ? Image { get; set; }
        public virtual Service ? Service { get; set; }
    }
}
