using AutoMapper;
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums.EnumBooking;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BookingService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;

        public BookingService(IUnitOfWork unitOfWork, ILogger<BookingService> logger, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<PaginatedList<GetBookingDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID, string customerName)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Booking> query = _unitOfWork.GetRepository<Booking>().Entities.Include(b=>b.Customer)
                 .Where(c => !c.DeletedTime.HasValue)
                 .OrderByDescending(c => c.CreatedTime);

            //Tìm theo ID
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy đặt phòng với ID đã nhập!");
                }
            }

            //Tìm theo khách hàng
            if (!string.IsNullOrWhiteSpace(customerID))
            {
                query = query.Where(r => r.CustomerId.ToString()==customerID);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy thông tin đặt phòng với ID khách hàng!");
                }
            }
            //Tìm theo tên khách hàng
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(r => r.Customer.Name.ToString().Equals(customerName));
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy thông tin đặt phòng của khách hàng: "+customerName);
                }
            }

            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetBookingDTO>(new List<GetBookingDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

        
            var responseItems = resultQuery.Select(booking => _mapper.Map<GetBookingDTO>(booking)).ToList();

            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetBookingDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }
        //public async Task<GetBookingDTO> CreateBooking(PostBookingDTO model)
        //{
        //    string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        //    if (String.IsNullOrWhiteSpace(model.CustomerId))
        //    {
        //        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID khách hàng");
        //    }
        //    if (model.CheckOutDate <= model.CheckInDate)
        //    {
        //        throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Ngày rời đi phải lớn hơn ngày đến");
        //    }

        //    //Insert Booking
        //    Booking booking = _mapper.Map<Booking>(model);

        //    booking.CreatedBy = userID;
        //    booking.LastUpdatedBy = userID;
        //    booking.CreatedTime = CoreHelper.SystemTimeNow;
        //    booking.LastUpdatedTime = CoreHelper.SystemTimeNow;
        //    booking.Status = EnumBooking.UNCONFIRMED;
        //    booking.TotalAmount = 0;
        //    await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);
        //    if (model.BookingDetails != null)
        //    {
        //        string bookingID = booking.Id;
        //        List<Room> rooms = await _unitOfWork.GetRepository<Room>().Entities.Where(r => r.DeletedTime == null && r.IsActive == true).ToListAsync();

        //        foreach (var item in model.BookingDetails)
        //        {
        //            var bookedRooms = await _unitOfWork.GetRepository<BookingDetail>().Entities
        //                                      .Where(bd => bd.Room.DeletedTime == null &&
        //                                                   bd.Room.IsActive == true &&
        //                                                   bd.Room.RoomTypeDetailId == item.RoomTypeDetailID &&
        //                                                   (bd.Booking.CheckInDate < booking.CheckOutDate && bd.Booking.CheckOutDate > booking.CheckInDate))
        //                                      .Select(bd => bd.RoomID)
        //                                      .ToListAsync();
        //            List<Room> availableRooms = rooms.Where(r => !bookedRooms.Contains(r.Id)).ToList();
        //            for (int i = 0; i < item.Quantity; i++)
        //            {
        //                //Lấy ra những phòng đầu tiên
        //                BookingDetail bookingDetail = new BookingDetail()
        //                {
        //                    BookingId = bookingID,
        //                    RoomID = availableRooms[i].Id,
        //                };
        //                await
        //            }

        //        }
        //    }
        //}
    }
}
