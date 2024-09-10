using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("RoomView")]
    public class RoomView
    {
        [StringLength(50)]
        public string RoomId { get; set; }

        [StringLength(50)]
        public string ViewHotelId { get; set; }

        public virtual Room Room { get; set; }
        public virtual ViewHotel ViewHotel { get; set; }

    }
}
