﻿using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class Employee : BaseEntity
    {
        [ForeignKey("Account")]
        public string AccountID { get; set; }
        public string Name { get; set; }
        public string IdentityCard { get; set; }
        public string Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; } 
        public DateTime HireDate { get; set; }
        public virtual Account? Account { get; set; }
        public virtual ICollection<Booking> ? Bookings { get; set; }
    }
}
