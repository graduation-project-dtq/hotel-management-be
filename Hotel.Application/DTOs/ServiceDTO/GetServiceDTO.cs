﻿
namespace Hotel.Application.DTOs.ServiceDTO
{
    public class GetServiceDTO
    {
        public string Id {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
