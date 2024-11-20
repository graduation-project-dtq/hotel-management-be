﻿using Hotel.Application.DTOs.ImageDTO;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.ServiceDTO
{
    public class PutServiceDTO
    {
        public string ? Name { get; set; } = string.Empty;
        public decimal ?Price { get; set; }
        public string? Description { get; set; } = string.Empty;
       
    }
}
