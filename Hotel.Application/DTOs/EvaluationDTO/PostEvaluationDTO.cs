﻿using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.ImageDTO;

namespace Hotel.Application.DTOs.EvaluationDTO
{
    public class PostEvaluationDTO
    {
        public string CustomerId { get; set; } = string.Empty;
        public string RoomTypeDetailId { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int Starts { get; set; }
        public virtual GetCustomerDTO? Customer { get; set; }
        public virtual ICollection<GetImage>? Images { get; set; }
    }
}
