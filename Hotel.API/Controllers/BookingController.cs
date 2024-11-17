using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Domain.Enums.EnumBooking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.API.Controllers
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Lấy danh sách đặt phòng
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="idSearch"></param>
        /// <param name="customerID"></param>
        /// <param name="customerName"></param>
        /// <param name="bookingDate"></param>
        /// <param name="checkInDate"></param>

        [HttpGet]
        public async Task<IActionResult> GetPageAsync(int index = 1, int pageSize = 10, string idSearch = "", string customerID = ""
            , string customerName = "", DateOnly? bookingDate = null, DateOnly ? checkInDate = null, EnumBooking? status = null, string phone = "")
        {
            PaginatedList<GetBookingDTO> result= await _bookingService.GetPageAsync(index, pageSize, idSearch, customerID, customerName,bookingDate,checkInDate, status, phone);
            return Ok(new BaseResponse<PaginatedList<GetBookingDTO>>(
              statusCode: StatusCodes.Status200OK,
              code: ResponseCodeConstants.SUCCESS,
              data: result,
              message: "Lấy danh sách đặt phòng thành công!"
           ));
        }

        /// <summary>
        /// Tạo đặt phòng mới
        /// </summary>
        /// <param name="model"></param>

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

        /// <summary>
        /// Cập nhật trạng thái của một đặt phòng  --Duyệt hoặc huỷ dựa theo Status hiện tại
        /// </summary>
        /// <param name="id"></param>
        /// 
        [HttpPut("UpdateStatus{id}")]
        public async Task<IActionResult> UpdateStatusBooking(string id  )
        {
            await _bookingService.UpdateStatusBooking(id);
            return Ok(new BaseResponse<string?>(
                statusCode: StatusCodes.Status200OK,
                code: ResponseCodeConstants.SUCCESS,
                data: null,
                message: "Xác nhận thành công!"
           ));
        }

        /// <summary>
        /// Lấy danh sách đặt phòng của một khách hàng
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="enumBooking"></param>
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

        //Thống kê theo ngày
        [HttpGet("date")]
        public async Task<IActionResult>  StatisticaInDate(DateOnly date)
        {
            StatisticaInDTO result= await _bookingService.StatisticaInDate(date);
            return Ok(new BaseResponse<StatisticaInDTO>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Thống kê thành công!"
            ));
        }

        //Thống kê theo tháng
        [HttpGet("mount")]
        public async Task<IActionResult> StatisticaInMount(int mount,int year)
        {
            StatisticaInDTO result = await _bookingService.StatisticaInMonth(mount,year);
            return Ok(new BaseResponse<StatisticaInDTO>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Thống kê thành công!"
            ));
        }

        //Thống kê theo tháng
        [HttpGet("year")]
        public async Task<IActionResult> StatisticaInYear( int year)
        {
            StatisticaInDTO result = await _bookingService.StatisticaInYear(year);
            return Ok(new BaseResponse<StatisticaInDTO>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: result,
               message: "Thống kê thành công!"
            ));
        }
        /// <summary>
        /// CheckIn tại khách sạn
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("CheckIn")]
        public async Task<IActionResult> CheckIn(CheckInDTO model)
        {
            await _bookingService.CheckIn(model);
            return Ok(new BaseResponse<string ?>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: null,
               message: "CheckIn thành công"
            ));
        }

        /// <summary>
        /// CheckOut tại khách sạn
        /// </summary>
        /// <param name="model"></param>
        [HttpPost("CheckOut")]
        public async Task<IActionResult> CheckOut(CheckOutDTO model)
        {
            await _bookingService.CheckOut(model);
            return Ok(new BaseResponse<string?>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: null,
               message: "CheckOut thành công"
            ));
        }

        [HttpPost("{idHuy}")]
        public async Task<IActionResult> HuyPhong(string idHuy)
        {
            await _bookingService.HuyPhong(idHuy);
            return Ok(new BaseResponse<string?>(
               statusCode: StatusCodes.Status200OK,
               code: ResponseCodeConstants.SUCCESS,
               data: null,
               message: "Huỷ phòng thành công"
            ));
        }

        
    }
}
