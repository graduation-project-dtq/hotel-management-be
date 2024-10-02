
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class FacilitiesRoom 
    {
        [ForeignKey("Room")]
        public string RoomID { get; set; } = string.Empty;

        [ForeignKey("Facilities")]
        public string FacilitiesID { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public virtual Room ? Room { get; set; }
        public virtual Facilities ? Facilities { get; set; }
    }
}
