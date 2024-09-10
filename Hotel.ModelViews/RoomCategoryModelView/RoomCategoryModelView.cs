using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.ModelViews.RoomCategoryModelView
{
    public class RoomCategoryModelView
    {
        public RoomCategoryModelView() { }
        public RoomCategoryModelView(string internalCode, string name  ,string description)
        {
            this.InternalCode = internalCode;
            this.Name = name;
            this.Description = description;
        }
        public string InternalCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
