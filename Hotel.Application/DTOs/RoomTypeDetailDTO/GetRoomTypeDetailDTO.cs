using Hotel.Application.DTOs.ImageDTO;
using Hotel.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.RoomTypeDetailDTO
{
    public class GetRoomTypeDetailDTO
    {
        public string Id {  get; set; } = string.Empty;
        public string ? RoomTypeID { get; set; }
        public string ? Name { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPrice {  get; set; }
        public int ? CapacityMax { get; set; }
        public decimal ? Area { get; set; }
        public string ? Description { get; set; }
        public float ? AverageStart { get; set; }
        
        public virtual ICollection<GetImageRoomTypeDetailDTO> ? ImageRoomTypeDetailDTOs {  get; set; }
    }
}
