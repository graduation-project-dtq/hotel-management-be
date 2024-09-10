using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    public class RoomTypeDetail : BaseEntity
    {
        [StringLength(50)]
        public string RoomCategoryId { get; set; }
        public string Name { get; set; }
        public int CapacityMax { get; set; }
        public string Image { get; set; }
        public decimal Area { get; set; }
        public string Amenities { get; set; }
        public string Furniture { get; set; }
        public string Rules { get; set; }
        public string Description { get; set; }
        public float AverageStart { get; set; }

        public virtual RoomCategory RoomCategory { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<RoomPrice> RoomPrices { get; set; }

    }
}
