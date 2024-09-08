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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RoomTypeDetail()
        {
            Bookings = new HashSet<Booking>();
            Evaluations = new HashSet<Evaluation>();
            Favorites = new HashSet<Favorite>();
            Rooms = new HashSet<Room>();
            RoomPrices = new HashSet<RoomPrice>();
        }
        [StringLength(50)]
        public string RoomCategoryId { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public int? CapacityMax { get; set; }

        [StringLength(255)]
        public string Image { get; set; }

        public decimal? Area { get; set; }

        [StringLength(255)]
        public string Amenities { get; set; }

        [StringLength(255)]
        public string Furniture { get; set; }

        [StringLength(255)]
        public string Rules { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Evaluation> Evaluations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Favorite> Favorites { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Room> Rooms { get; set; }

        public virtual RoomCategory RoomCategory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomPrice> RoomPrices { get; set; }
    }
}
