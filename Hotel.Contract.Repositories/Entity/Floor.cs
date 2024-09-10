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
    [Table("Floor")]
    public class Floor : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }

    }
}
