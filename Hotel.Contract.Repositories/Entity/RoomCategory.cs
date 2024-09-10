using Hotel.Core.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hotel.Contract.Repositories.Entity
{
    [Table("RoomCategory")]

    public class RoomCategory : BaseEntity
    {
      
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<RoomTypeDetail> RoomTypeDetails { get; set; }

    }
}
