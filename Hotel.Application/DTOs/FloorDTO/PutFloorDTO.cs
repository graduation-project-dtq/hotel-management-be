

using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.FloorDTO
{
    public class PutFloorDTO
    {
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string  Name {  get; set; }
    }
}
