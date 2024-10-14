using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.BookingDTO
{
    public class GetPunishesDTO
    {
       
        public string FacilitiesName { get; set; } = string.Empty; //tên
        public int Quantity { get; set; }//Số lượng
        public decimal Fine { get; set; }//Tiền phạt
    }
}
