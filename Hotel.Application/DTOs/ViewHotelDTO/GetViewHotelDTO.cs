﻿namespace Hotel.Application.DTOs.ViewHotelDTO
{
    public class GetViewHotelDTO
    {
        public string Id {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}