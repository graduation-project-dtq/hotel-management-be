using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("Room")]
    public class Room : BaseEntity
    {

        [StringLength(50)]
        public string FloorId { get; set; }
        [StringLength(50)]
        public string RoomTypeDetailId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }

        public virtual Floor Floor { get; set; }
        public virtual RoomTypeDetail RoomTypeDetail { get; set; }
        public virtual  ICollection<BookingDetail> BookingDetails { get; set; }
        public virtual ICollection<RoomView> RoomViews { get; set; }
        public virtual ICollection<FacilitiesRoom> FacilitiesRooms { get; set; }


    }
}
