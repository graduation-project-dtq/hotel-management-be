

using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Domain.Entities
{
    public  class Customer : BaseEntity
    {
        [ForeignKey("Account")]
        public string AccountID { get; set; }
        public string Name { get; set; }
        public string? IdentityCard { get; set; }
        public string? Sex  { get; set; } 
        public DateOnly ? DateOfBirth { get; set; }
        public string ? Phone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string ?Address { get; set; }
        public int? CredibilityScore { get; set; } 
        public int ? AccumulatedPoints { get; set; } //Điểm tích luỹ
        public virtual Account ? Account {  get; set; }
        public virtual ICollection<Booking> ? Bookings { get; set; }
        public virtual ICollection<Evaluation> ? Evaluations { get; set; }
        public virtual ICollection<Notification> ? Notification { get; set; }
        public virtual ICollection<Voucher>? Vouchers { get; set; }
    }
}
