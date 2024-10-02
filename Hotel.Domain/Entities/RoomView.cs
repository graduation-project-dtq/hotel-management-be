using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class RoomView 
    {
        [ForeignKey("Room")]
        public string RoomId { get; set; } = string.Empty;

        [ForeignKey("ViewHotel")]
        public string ViewHotelId { get; set; } = string.Empty;

        public virtual Room ? Room { get; set; }
        public virtual ViewHotel ? ViewHotel { get; set; }
    }
}
