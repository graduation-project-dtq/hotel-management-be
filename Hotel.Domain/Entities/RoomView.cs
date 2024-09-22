
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class RoomView 
    {
        [ForeignKey("Room")]
        public string RoomId { get; set; }

        [ForeignKey("ViewHotel")]
        public string ViewHotelId { get; set; }

        public virtual Room ? Room { get; set; }
        public virtual ViewHotel ? ViewHotel { get; set; }
    }
}
