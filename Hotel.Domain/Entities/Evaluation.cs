﻿using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Evaluation : BaseEntity
    {
        [ForeignKey("Customer")]
        public string CustomerId { get; set; } = string.Empty;

        [ForeignKey("RoomTypeDetail")]
        public string RoomTypeDetailId { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Starts { get; set; }
        public virtual Customer ? Customer { get; set; }
        public virtual RoomTypeDetail ? RoomTypeDetail { get; set; }

        public virtual ICollection<ImageEvaluation> ? ImageEvaluations { get; set; }
    }
}
