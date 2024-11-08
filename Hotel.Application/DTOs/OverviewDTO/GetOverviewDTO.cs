

using Hotel.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Application.DTOs.OverviewDTO
{
    public class GetOverviewDTO
    {
        public string Id {  get; set; }=string.Empty;
        public float EmployeePoint { get; set; }  //Đánh giá nhân viên
        public float ComfortPoint { get; set; } //Đánh giá tiện nghi
        public float ClearPoint { get; set; } //Sạch sẽ
        public float ServicePoint { get; set; } //Dịch vụ
        public float ViewPoint { get; set; } //View
        public float RoomPoint { get; set; } //Phòng

 
        public string ? CustomerId { get; set; } 
    }
}
