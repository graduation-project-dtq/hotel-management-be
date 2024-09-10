using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{

    [Table("ServiceBooking")]
    public class ServiceBooking
    {
        [StringLength(50)]
        public string BookingID { get; set; }

        [StringLength(50)]
        public string ServiceID { get; set; }
        public int Quantity { get; set; }
        public float Total { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Service Service { get; set; }

    }
}
