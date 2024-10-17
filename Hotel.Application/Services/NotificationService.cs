using AutoMapper;
using Hotel.Application.DTOs.NotificationDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }
        public async Task<List<GetNotificationDTO>> GetByCustomerId(string customerId)
        {
            if(String.IsNullOrWhiteSpace(customerId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng không để trống Id khách hàng");
            }
            //Lấy danh sách thông báo cho một khách hàng 
            List<Notification> notifications = await _unitOfWork.GetRepository<Notification>().Entities.Where(n=>n.CustomerId == customerId && n.DeletedTime == null).ToListAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không có thông báo nào cho khách hàng");

            List<GetNotificationDTO> notificationDTO = _mapper.Map<List<GetNotificationDTO>>(notifications);
            return notificationDTO;
        }
        public async Task CreateNotification(PostNotificationDTO model)
        {
            //Kiểm tra có tồn tại khách hàng hay không
            Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(model.CustomerId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tồn tại khách hàng có mã khách hàng"+ model.CustomerId);
            
            //Mapping
            Notification notification = _mapper.Map<Notification>(model);

            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            notification.CreatedBy = userId;
            notification.LastUpdatedBy=userId;

            await _unitOfWork.GetRepository<Notification>().InsertAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteNotification(string id)
        {
            if(String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng không để trống Id");

            }
            Notification notification = await _unitOfWork.GetRepository<Notification>().Entities.Where(n=>n.Id== id && n.DeletedTime == null).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không có thông báo nào có Id :" + id);
            
            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            notification.DeletedBy = userId;
            notification.DeletedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Notification>().UpdateAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
