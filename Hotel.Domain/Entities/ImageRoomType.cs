using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hotel.Domain.Entities
{
    public class ImageRoomType 
    {
        [ForeignKey("RoomType")]
        public string RoomTypeID { get; set; } = string.Empty;

        [ForeignKey("Image")]
        public string ImageID { get; set; } = string.Empty;

        [JsonIgnore]
        public virtual RoomType? RoomType { get; set; }
        [JsonIgnore]
        public virtual Image? Image { get; set; }
    }
}
