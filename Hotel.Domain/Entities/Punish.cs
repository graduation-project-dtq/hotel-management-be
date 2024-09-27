using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Punish 
    {
        [ForeignKey("Booking")]
        public string BookingID {  get; set; }

        [ForeignKey("Facilities")]
        public string FacilitiesID {  get; set; }
        public string ? Note {  get; set; }
        public decimal ? Fine {  get; set; } //Tiền phạt

        public virtual Booking ? Booking {  get; set; }
        public virtual Facilities ? Facilities { get; set; }
    }
}
