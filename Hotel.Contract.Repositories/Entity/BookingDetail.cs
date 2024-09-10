﻿using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("BookingDetail")]
    public class BookingDetail 
    {
        [StringLength(50)]    
        public string BookingId { get; set; }

        [StringLength(50)]

        public string RoomId { get; set; }
        public decimal Price { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual Room Room { get; set; }


    }
}
