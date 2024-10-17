namespace Hotel.Application.DTOs.NotificationDTO
{
    public class PutNotificationDTO
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
