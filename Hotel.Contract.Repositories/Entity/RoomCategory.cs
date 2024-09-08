using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("RoomCategory")]

    public class RoomCategory : BaseEntity
    {
        public RoomCategory()
        {
            RoomTypeDetails = new HashSet<RoomTypeDetail>();
        }
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        public string InternalCode { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RoomTypeDetail> RoomTypeDetails { get; set; }
    }
}
