using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.ModelViews.RoomTypeDetailsMovelView
{
    public class RoomTypeDetailMovelView
    {
        public string InternalCode {  get; set; }
        public string RoomCategoryId { get; set; }
        public string Name { get; set; }
        public int CapacityMax { get; set; }
        public string Image { get; set; }
        public decimal Area { get; set; }
        public string Amenities { get; set; }
        public string Furniture { get; set; }
        public string Rules { get; set; }
        public string Description { get; set; }
    }
}
