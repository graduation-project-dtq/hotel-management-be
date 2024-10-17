
using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public class Notification : BaseEntity
    {
        [ForeignKey("Customer")]
        public string CustomerId {  get; set; } = string.Empty;
        public string Title {  get; set; } = string.Empty;
        public string Content {  get; set; } = string.Empty;
        public virtual Customer ? Customer { get; set; }
    }
}
