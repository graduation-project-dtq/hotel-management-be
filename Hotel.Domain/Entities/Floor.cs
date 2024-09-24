﻿

using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class Floor : BaseEntity
    {
        public string Name {  get; set; }
        public virtual ICollection<Room> ? Rooms { get; set; }
    }
}