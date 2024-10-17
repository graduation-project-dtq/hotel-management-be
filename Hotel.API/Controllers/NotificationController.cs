using Hotel.Application.DTOs.NotificationDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }
        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetByCustomerId(string customerId)
        {
            List<GetNotificationDTO> result = await _notificationService.GetByCustomerId(customerId);
            return Ok(new BaseResponse<List<GetNotificationDTO>>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: result,
             message: "Lấy danh sách thông báo thành công!"
          ));
        }
        [HttpPost]
        public async Task<IActionResult> CreateNotification(PostNotificationDTO model)
        {
            await _notificationService.CreateNotification(model);
            return Ok(new BaseResponse<string ?>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: null,
             message: "Tạo thông báo thành công!"
          ));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNotification(string id)
        {
            await _notificationService.DeleteNotification(id);
            return Ok(new BaseResponse<string?>(
             statusCode: StatusCodes.Status200OK,
             code: ResponseCodeConstants.SUCCESS,
             data: null,
             message: "Xoá thông báo thành công!"
          ));
        }
    }
}
