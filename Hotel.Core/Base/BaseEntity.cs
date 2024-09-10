﻿using Hotel.Core.Utils;
using System.ComponentModel.DataAnnotations;
namespace Hotel.Core.Base
{
    public  class BaseEntity
    {
        protected BaseEntity()
        {
            Id = Guid.NewGuid().ToString("N");
            CreatedTime = LastUpdatedTime = CoreHelper.SystemTimeNow;
        }

        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        public string ? InternalCode { get; set; }
        public string ? CreatedBy { get; set; }
        public string ? LastUpdatedBy { get; set; }
        public string ? DeletedBy { get; set; }
        public DateTimeOffset CreatedTime { get; set; } 
        public DateTimeOffset LastUpdatedTime { get; set; } 
        public DateTimeOffset? DeletedTime { get; set; }
    }
}