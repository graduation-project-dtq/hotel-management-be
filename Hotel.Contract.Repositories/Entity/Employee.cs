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
        public Employee()
        {
            Bookings = new HashSet<Booking>();
            Users = new HashSet<User>();
        }
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string IdentityCard { get; set; }

        [StringLength(10)]
        public string Sex { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(11)]
        public string Phone { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [Column(TypeName = "date")]
        public DateTime? HireDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Bookings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }
    }
}
