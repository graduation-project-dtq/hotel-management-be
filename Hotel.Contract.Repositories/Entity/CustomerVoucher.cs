using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("CustomerVoucher")]
    public class CustomerVoucher : BaseEntity
    {

        [Column(Order = 0)]
        [StringLength(50)]
        public string CustomerId { get; set; }

   
        [Column(Order = 1)]
        [StringLength(50)]
        public string VoucherId { get; set; }

        public DateTime? UsedDate { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Voucher Voucher { get; set; }
    }
}
