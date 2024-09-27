using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class ImageRoomTypeDetail 
    {
        [ForeignKey("RoomTypeDetail")]
        public string RoomTypeDetailID {  get; set; }

        [ForeignKey("Image")]
        public string ImageID {  get; set; }
        public virtual RoomTypeDetail? RoomTypeDetail {  get; set; }
        public virtual Image ? Image { get; set; }
    }
}
