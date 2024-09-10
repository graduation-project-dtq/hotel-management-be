using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("RoomCategory")]

    public class RoomCategory : BaseEntity
    {
        public RoomCategory()
        {
            RoomTypeDetails = new HashSet<RoomTypeDetail>();
        }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<RoomTypeDetail> RoomTypeDetails { get; set; }
    }
}
