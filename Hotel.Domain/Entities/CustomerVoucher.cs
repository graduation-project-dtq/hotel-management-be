using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class CustomerVoucher
    {
        [ForeignKey("Customer")]
        public string CustomerId { get; set; }

        [ForeignKey("Voucher")]
        public string VoucherID { get; set; }
        public DateTime ? UsedDate { get; set; }
        public virtual Customer ? Customer { get; set; }
        public virtual Voucher ? Voucher { get; set; }
    }
}
