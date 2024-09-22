using Hotel.Core.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public bool IsAdmin { get; set; }
        public bool IsActive {  get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public string? EmailCode { get; set; }
        public DateTime? CodeGeneratedTime { get; set; }
        public virtual ICollection<Employee> ? Employees { get; set; }
        public virtual ICollection<Customer> ? Customers { get; set; }
        // Navigation properties
      
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
            NormalizedUserName = UserName?.ToUpper();
            NormalizedEmail = Email?.ToUpper();
            SecurityStamp = Guid.NewGuid().ToString();
            EmailConfirmed = true;
        }

    }
}
