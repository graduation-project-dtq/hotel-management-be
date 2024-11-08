using Hotel.Core.Base;

namespace Hotel.Domain.Entities
{
    public class Overview : BaseEntity
    {
      
        public float EmployeePoint {  get; set; }  //Đánh giá nhân viên
        public float ComfortPoint {  get; set; } //Đánh giá tiện nghi
        public float ClearPoint {  get; set; } //Sạch sẽ
        public float ServicePoint {  get; set; } //Dịch vụ
        public float ViewPoint {  get; set; } //View
        public float RoomPoint {  get; set; } //Phòng
        public string CustomerId { get; set; } = string.Empty;
        public virtual Customer? Customer { get; set; }
    }
}
