using Hotel.Application.DTOs.NotificationDTO;

namespace Hotel.Application.Interfaces
{
    public interface INotificationService
    {
        Task<List<GetNotificationDTO>> GetByCustomerId(string customerId);
        Task CreateNotification(PostNotificationDTO model);
        Task DeleteNotification(string id);
    }
}
