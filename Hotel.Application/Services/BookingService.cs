using AutoMapper;
using Hotel.Application.DTOs.BookingDetailDTO;
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
        private readonly IRoomService _roomService;

        
        public BookingService(IUnitOfWork unitOfWork, ILogger<BookingService> logger, IMapper mapper, IHttpContextAccessor contextAccessor, IRoomService roomService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _roomService = roomService;
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
        public async Task<GetBookingDTO> CreateBooking(PostBookingDTO model)
        {
            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            // Kiểm tra ID khách hàng
            if (string.IsNullOrWhiteSpace(model.CustomerId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID khách hàng");
            }

            // Kiểm tra ngày đến và ngày rời đi
            if (model.CheckOutDate <= model.CheckInDate)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Ngày rời đi phải lớn hơn ngày đến");
            }

            // Insert Booking
            Booking booking = new Booking
            {
                CustomerId = model.CustomerId,
                EmployeeId = null, // Giả sử không có nhân viên
                CreatedBy = userID,
                LastUpdatedBy = userID,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow,
                Status = EnumBooking.UNCONFIRMED,
                TotalAmount = 0,
                BookingDate = DateOnly.FromDateTime(CoreHelper.SystemTimeNow.Date),
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                BookingDetails = new List<BookingDetail>()
            };

            await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            if (model.BookingDetails != null && model.BookingDetails.Count > 0)
            {
                string bookingID = booking.Id;

                foreach (var item in model.BookingDetails)
                {
                    // Kiểm tra xem có phòng trống hay không
                    List<GetRoomDTO> listRoomActive = await _roomService.FindRoomBooking(model.CheckInDate, model.CheckOutDate, item.RoomTypeDetailID);
                    if (listRoomActive == null || listRoomActive.Count == 0)
                    {
                        //Xoá dữ liệu
                        //Xoá   
                        Booking bkDelete = await _unitOfWork.GetRepository<Booking>().GetByIdAsync(bookingID);
                        
                        List<BookingDetail> listDetail = await _unitOfWork.GetRepository<BookingDetail>().Entities.Where(bd=>bd.BookingId==bkDelete.Id).ToListAsync();

                        if (listDetail != null && listDetail.Count > 0)
                        {
                            foreach(var bd in listDetail)
                            {
                                await _unitOfWork.GetRepository<BookingDetail>().DeleteAsync(bd.BookingId);
                                await _unitOfWork.SaveChangesAsync();
                            }
                            await _unitOfWork.GetRepository<Booking>().DeleteAsync(bkDelete.Id);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không còn phòng nào trống!");
                    }

                    string roomID = listRoomActive[0].Id;

                    // Tạo BookingDetail mới
                    BookingDetail bookingDetail = new BookingDetail()
                    {
                        BookingId = bookingID,
                        RoomID = roomID,
                    };
                    booking.BookingDetails.Add(bookingDetail);
                    await _unitOfWork.GetRepository<BookingDetail>().InsertAsync(bookingDetail);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Trả dữ liệu ra
            GetBookingDTO getBookingDTO = new GetBookingDTO
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                // Chuyển đổi từ DateTime sang DateOnly
                BookingDate = DateOnly.FromDateTime(booking.CreatedTime.DateTime),
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                BookingDetail = new List<GetBookingDetailDTO>()
            };

            // Thêm thông tin BookingDetail vào DTO
            foreach (var item in booking.BookingDetails)
            {
                if (item != null)
                {
                    // Gán dữ liệu cho GetBookingDetailDTO
                    GetBookingDetailDTO getBookingDetail = new GetBookingDetailDTO
                    {
                        RoomID = item.RoomID,
                    };
                    getBookingDTO.BookingDetail.Add(getBookingDetail);
                }
            }

            return getBookingDTO;
        }
    }
}
