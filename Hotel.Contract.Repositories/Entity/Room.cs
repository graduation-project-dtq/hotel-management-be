using Hotel.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Contract.Repositories.Entity
{
    public class Room:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
