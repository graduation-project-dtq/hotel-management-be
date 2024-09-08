
using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("BillDetail")]
    public class BillDetail
    {
        [Key, Column(Order = 0)] // Đánh dấu khóa chính
        [StringLength(50)]
        public string BillId { get; set; }

        [Key, Column(Order = 1)] // Đánh dấu khóa chính kết hợp
        [StringLength(50)]
        public string ServiceId { get; set; }

        public int? Quantity { get; set; }
        public virtual Service Service { get; set; }
    }
}
