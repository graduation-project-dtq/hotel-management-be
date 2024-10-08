

using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class VoucherRoomTypeDetail
    {
        [ForeignKey("Voucher")]
        public string VoucherID {  get; set; } = string.Empty;
        [ForeignKey("RoomTypeDetail")]
        public string RoomTypeDetailID {  get; set; } =string.Empty;
        public virtual Voucher ? Voucher {  get; set; }
        public virtual RoomTypeDetail? RoomTypeDetail { get; set; }
    }
}
