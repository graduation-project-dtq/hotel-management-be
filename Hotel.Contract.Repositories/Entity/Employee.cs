using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Hotel.Contract.Repositories.Entity
{
    [Table("Employee")]
    public class Employee : BaseEntity
    {
        public string Name { get; set; }
        public string IdentityCard { get; set; }
        public string Sex { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime HireDate { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }

}
