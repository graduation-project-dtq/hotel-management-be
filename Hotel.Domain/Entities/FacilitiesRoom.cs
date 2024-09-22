
using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class FacilitiesRoom : BaseEntity
    {
        [ForeignKey("Room")]
        public string RoomID { get; set; }

        [ForeignKey("Facilities")]
        public string FacilitiesID { get; set; }
        public int Quantity { get; set; }
        public virtual Room ? Room { get; set; }
        public virtual Facilities ? Facilities { get; set; }
    }
}
