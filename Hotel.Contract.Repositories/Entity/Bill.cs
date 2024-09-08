using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Hotel.Contract.Repositories.Entity
{
    [Table("Bill")]
    public class Bill: BaseEntity
    {
        
        [StringLength(50)]
        public string EmployeeId { get; set; }

        [StringLength(50)]
        public string BookingId { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}
