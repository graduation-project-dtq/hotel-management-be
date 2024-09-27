

using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace Hotel.Domain.Entities
{
    public  class Customer : BaseEntity
    {
        [ForeignKey("Account")]
        public string AccountID { get; set; }
        public string Name { get; set; }
        public int? IdentityCard { get; set; }
        public string? Sex  { get; set; } 
        public DateTime? DateOfBirth { get; set; }
        public string ? Phone { get; set; }
        public string Email { get; set; }
        public string ?Address { get; set; }
        public int? CredibilityScore { get; set; } 
        public int ? AccumulatedPoints { get; set; } //Điểm tích luỹ
        public virtual Account ? Account {  get; set; }
        public virtual ICollection<Booking> ? Bookings { get; set; }
        public virtual ICollection<CustomerVoucher> ? CustomerVouchers { get; set; }
        public virtual ICollection<Evaluation> ? Evaluations { get; set; }
        public virtual ICollection<Notification> ? Notification { get; set; }
    }
}
