
using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Notification : BaseEntity
    {
        [ForeignKey("Customer")]
        public string CustomerId {  get; set; }
        public string Content {  get; set; }

        public virtual Customer ? Customer { get; set; }
    }
}
