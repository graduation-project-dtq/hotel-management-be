using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class ImageRoomTypeDetail : BaseEntity
    {
        [ForeignKey("RoomTypeDetail")]
        public string RoomTypeDetailID {  get; set; }

        [ForeignKey("Image")]
        public string ImageID {  get; set; }
        public virtual RoomTypeDetail? RoomTypeDetail {  get; set; }
        public virtual Image ? Image { get; set; }
    }
}
