using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Enums.EnumBooking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Hotel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "", string customerID = "", string customerName = "")
        {
            PaginatedList<GetBookingDTO> result= await _bookingService.GetPageAsync(index, pageSize, idSearch, customerID, customerName);
            return Ok(new BaseResponse<PaginatedList<GetBookingDTO>>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: result,
              message: "Lấy danh sách đặt phòng thành công!"
           ));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking(PostBookingDTO model)
        {
            
            GetBookingDTO result = await _bookingService.CreateBooking(model);
            return Ok(new BaseResponse<GetBookingDTO>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: result,
              message: "Đặt phòng thành công!"
             ));
        }
        [HttpPut("UpdateStatus")]
        public async Task<IActionResult> UpdateStatusBooking(string bookingID, EnumBooking enumBooking)
        {
            await _bookingService.UpdateStatusBooking(bookingID, enumBooking);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Xác nhận thành công!"
           ));
        }

        [HttpGet("GetBooking")]
        public async Task<IActionResult> GetBookingByCustomerId(string customerId, EnumBooking enumBooking)
        {
            List<GetBookingDTO> result = await _bookingService.GetBookingByCustomerId(customerId, enumBooking);
            return Ok(new BaseResponse<List<GetBookingDTO>>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: result,
                message: "Lấy danh sách đặt phòng thành công!"
             ));
        }
    }
}
