namespace Hotel.Domain.Enums.EnumBooking
{
    public enum EnumBooking
    {
        CONFIRMED = 1,           // Đã xác nhận
        CANCELED = 2,            // Đã hủy
        CANCELLATIONREQUEST = 0,  // Yêu cầu hủy
        UNCONFIRMED = 3,         // Chưa xác nhận
        CHECKEDIN = 4,           // Đã check-in
        CHECKEDOUT = 5           // Đã check-out
    }
}
