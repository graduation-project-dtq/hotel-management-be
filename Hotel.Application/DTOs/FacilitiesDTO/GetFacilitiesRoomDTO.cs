using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.FacilitiesDTO
{
    public class GetFacilitiesRoomDTO : GetFacilitiesDTO
    {
        public string roomId { get; set; } = string.Empty;
       
        public int Quantity { get; set; }
    }
}
