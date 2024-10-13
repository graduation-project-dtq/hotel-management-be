using AutoMapper;
using Hotel.Application.DTOs.BookingDetailDTO;
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.DTOs.ServiceDTO;
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
using Microsoft.Extensions.Configuration;
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
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IRoomTypeDetailService _roomTypeDetailService;
        public BookingService(IUnitOfWork unitOfWork, ILogger<BookingService> logger, IMapper mapper,
            IHttpContextAccessor contextAccessor, IRoomService roomService, IEmailService emailService
            , IConfiguration configuration, IRoomTypeDetailService roomTypeDetailService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _roomService = roomService;
            _emailService = emailService;
            _configuration = configuration;
            _roomTypeDetailService = roomTypeDetailService;
        }

        public async Task<PaginatedList<GetBookingDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID, string customerName, DateOnly? bookingDate, DateOnly? checkInDate)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Booking> query = _unitOfWork.GetRepository<Booking>().Entities.Include(b => b.Customer)
                 .Include(c => c.Customer)
                 .Include(bk => bk.BookingDetails)
                 .Include(bk => bk.ServiceBookings)
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
                query = query.Where(r => r.CustomerId.ToString() == customerID);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy thông tin đặt phòng với ID khách hàng!");
                }
            }
            //Tìm theo tên khách hàng
            if (!string.IsNullOrWhiteSpace(customerName))
            {
                query = query.Where(r => r.Customer.Name.ToString().Contains(customerName));
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy thông tin đặt phòng của khách hàng: " + customerName);
                }
            }
            //Tìm theo ngày đặt phòng
            if (bookingDate != null)
            {
                query = query.Where(bk => DateOnly.FromDateTime(bk.CreatedTime.Date) == bookingDate);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy thông tin đặt phòng vào ngày: " + bookingDate.Value.ToString());
                }
            }
            //Tìm theo ngày checkin
            if (checkInDate != null)
            {
                query = query.Where(bk => bk.CheckInDate == checkInDate);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìn thấy lịch sử đặt phòng có ngày checkin là: " + checkInDate.ToString());
                }
            }
            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetBookingDTO>(new List<GetBookingDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();


            List<GetBookingDTO> responseItems = new List<GetBookingDTO>();
            foreach (Booking item in query)
            {
                GetBookingDTO bookingModel = new GetBookingDTO();

                bookingModel.Id = item.Id;
                bookingModel.EmployeeId = item.EmployeeId;
                bookingModel.CustomerId = item.CustomerId;
                bookingModel.CustomerName = item.Customer != null ? item.Customer.Name : null;
                bookingModel.PhoneNumber = item.PhoneNumber;
                bookingModel.PromotionalPrice = item.PromotionalPrice;
                bookingModel.Deposit = item.Deposit;
                bookingModel.BookingDate = DateOnly.FromDateTime(item.CreatedTime.Date).ToString("dd/MM/yyyy");
                bookingModel.TotalAmount = item.TotalAmount;
                bookingModel.UnpaidAmount = item.UnpaidAmount;
                bookingModel.CheckInDate = item.CheckInDate.ToString("dd/MM/yyyy");
                bookingModel.CheckOutDate = item.CheckOutDate.ToString("dd/MM/yyyy");
                bookingModel.BookingDetail = new List<GetBookingDetailDTO>();
                bookingModel.Services = new List<GetServiceBookingDTO>();

                if (item.BookingDetails.Count > 0)
                {
                    foreach (BookingDetail bookingDetail in item.BookingDetails)
                    {
                        Room room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(bookingDetail.RoomID);
                        GetBookingDetailDTO bookingDetailDTO = new GetBookingDetailDTO()
                        {
                            RoomName = room.Name,
                        };
                        bookingModel.BookingDetail.Add(bookingDetailDTO);
                    }
                }
                if (item.ServiceBookings.Count > 0)
                {
                    foreach (ServiceBooking serviceBooking in item.ServiceBookings)
                    {
                        Service service = await _unitOfWork.GetRepository<Service>().GetByIdAsync(serviceBooking.ServiceID);
                        GetServiceBookingDTO serviceBookingDTO = new GetServiceBookingDTO()
                        {
                            ServiceName = service.Name,
                            Quantity = serviceBooking.Quantity,
                        };
                        bookingModel.Services.Add(serviceBookingDTO);
                    }
                }
                responseItems.Add(bookingModel);
            }

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
        public async Task<List<GetBookingDTO>> GetBookingByCustomerId(string customerId, EnumBooking enumBooking)
        {
            if (String.IsNullOrWhiteSpace(customerId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID khách hàng");
            }
            List<Booking> bookings = await _unitOfWork.GetRepository<Booking>().Entities
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                .Include(b => b.ServiceBookings)
                .Where(b => b.CustomerId == customerId && b.Status == enumBooking && b.DeletedTime == null).ToListAsync();

            List<GetBookingDTO> getBookingDTO = new List<GetBookingDTO>();

            foreach (Booking item in bookings)
            {
                GetBookingDTO bookingModel = new GetBookingDTO();

                bookingModel.Id = item.Id;
                bookingModel.EmployeeId = item.EmployeeId;
                bookingModel.CustomerId = item.CustomerId;
                bookingModel.CustomerName = item.Customer != null ? item.Customer.Name : null;
                bookingModel.PhoneNumber = item.PhoneNumber;
                bookingModel.PromotionalPrice = item.PromotionalPrice;
                bookingModel.Deposit = item.Deposit;
                bookingModel.BookingDate = DateOnly.FromDateTime(item.CreatedTime.Date).ToString("dd/MM/yyyy HH:mm:ss");
                bookingModel.TotalAmount = item.TotalAmount;
                bookingModel.UnpaidAmount = item.UnpaidAmount;
                bookingModel.CheckInDate = item.CheckInDate.ToString("dd/MM/yyyy");
                bookingModel.CheckOutDate = item.CheckOutDate.ToString("dd/MM/yyyy");
                bookingModel.BookingDetail = new List<GetBookingDetailDTO>();
                bookingModel.Services = new List<GetServiceBookingDTO>();

                if (item.BookingDetails.Count > 0)
                {
                    foreach (BookingDetail bookingDetail in item.BookingDetails)
                    {
                        Room room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(bookingDetail.RoomID);
                        GetBookingDetailDTO bookingDetailDTO = new GetBookingDetailDTO()
                        {
                            RoomName = room.Name,
                        };
                        bookingModel.BookingDetail.Add(bookingDetailDTO);
                    }
                }
                if (item.ServiceBookings.Count > 0)
                {
                    foreach (ServiceBooking serviceBooking in item.ServiceBookings)
                    {
                        Service service = await _unitOfWork.GetRepository<Service>().GetByIdAsync(serviceBooking.ServiceID);
                        GetServiceBookingDTO serviceBookingDTO = new GetServiceBookingDTO()
                        {
                            ServiceName = service.Name,
                            Quantity = serviceBooking.Quantity,
                        };
                        bookingModel.Services.Add(serviceBookingDTO);
                    }
                }
                getBookingDTO.Add(bookingModel);
            }
            return getBookingDTO;
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

            Customer exitCustomer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(model.CustomerId)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Khách hàng không tồn tại");
            if (String.IsNullOrWhiteSpace(model.PhoneNumber))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống số điện thoại khách hàng");

            }

            Voucher voucher = new Voucher()
            {
                Id= null,
            };
            if (!String.IsNullOrWhiteSpace(model.VoucherId))
            {
                voucher = await _unitOfWork.GetRepository<Voucher>().Entities
                   .Where(v => v.Id == model.VoucherId && v.DeletedTime != null
                   && v.StartDate <= CoreHelper.SystemDateOnly && v.EndDate >= CoreHelper.SystemDateOnly
                   && v.Quantity > 0).FirstOrDefaultAsync()
                   ?? throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Voucher không khả dụng!");
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
                Deposit = model.Deposit,
                PhoneNumber = model.PhoneNumber,
                UnpaidAmount = 0,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                BookingDetails = new List<BookingDetail>(),
                ServiceBookings = new List<ServiceBooking>(),
                //PromotionalPrice = voucher.DiscountAmount,
                //VoucherId = voucher.Id,
                PromotionalPrice = 0,
                VoucherId = null,
            };

            await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            string bookingID = booking.Id;
            if (model.BookingDetails != null && model.BookingDetails.Count > 0)
            {
                foreach (var item in model.BookingDetails)
                {
                    // Kiểm tra xem có phòng trống hay không
                    FindRoomDTO findRoomDTO = new FindRoomDTO()
                    {
                        CheckInDate = model.CheckInDate,
                        CheckOutDate = model.CheckOutDate,
                        RoomTypeDetailID = item.RoomTypeDetailID,
                    };
                    List<GetRoomDTO> listRoomActive = await _roomService.FindRoomBooking(findRoomDTO);
                    if (listRoomActive == null )
                    {
                        //Xoá dữ liệu
                        //Xoá   
                        await DeleteBookingAsync(bookingID);
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

                    //Tính tiền
                    decimal price = await _roomTypeDetailService.GetDiscountPrice(bookingDetail.Room.RoomTypeDetailId);
                    if (price != 0)
                    {
                        booking.TotalAmount += price;
                    }
                    else
                    {
                        Room room = await _unitOfWork.GetRepository<Room>().Entities
                            .Include(r => r.RoomTypeDetail)
                            .Where(r => r.Id == bookingDetail.RoomID)
                            .FirstOrDefaultAsync();
                        if (room != null)
                        {
                            booking.TotalAmount += room.RoomTypeDetail.BasePrice;
                        }
                    }
                }
            }
            //Thêm dịch vụ
            if (model.Services != null && model.Services.Count > 0)
            {
                foreach (PostServiceBookingDTO item in model.Services)
                {
                    Service initService = await _unitOfWork.GetRepository<Service>().GetByIdAsync(item.ServiceID);
                    if (initService == null)
                    {
                        await DeleteBookingAsync(bookingID);
                        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Dịch vụ không tồn tại!");
                    }

                    ServiceBooking service = new ServiceBooking()
                    {
                        BookingID = booking.Id,
                        ServiceID = item.ServiceID,
                        Quantity = item.Quantity,
                    };

                    await _unitOfWork.GetRepository<ServiceBooking>().InsertAsync(service);

                    //Tính tiền dịch vụ
                    booking.TotalAmount += service.Service.Price * service.Quantity;
                }
            }

            booking.UnpaidAmount = booking.TotalAmount;
            if (booking.Deposit > 0)
            {
                booking.UnpaidAmount = booking.TotalAmount - booking.Deposit;
            }
            //Trừ số lượng voucher
            voucher.Quantity = voucher.Quantity - 1;
            booking.TotalAmount = booking.TotalAmount - booking.PromotionalPrice;

            if (voucher.Quantity == 0)
            {
                await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
                await _unitOfWork.SaveChangesAsync();
            }
            await _unitOfWork.SaveChangesAsync();

            // Trả dữ liệu ra
            GetBookingDTO getBookingDTO = new GetBookingDTO
            {
                Id = booking.Id,
                CustomerId = booking.CustomerId,
                Deposit = booking.Deposit,
                PromotionalPrice = booking.PromotionalPrice,
                TotalAmount = booking.TotalAmount,
                UnpaidAmount = booking.UnpaidAmount,
                BookingDate = booking.CreatedTime.DateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                CheckInDate = booking.CheckInDate.ToString("dd/MM/yyyy"),
                CheckOutDate = booking.CheckOutDate.ToString("dd/MM/yyyy"),
                PhoneNumber = booking.PhoneNumber,
                BookingDetail = new List<GetBookingDetailDTO>(),
                Services = new List<GetServiceBookingDTO>()
            };

            // Thêm thông tin BookingDetail vào DTO
            foreach (var item in booking.BookingDetails)
            {
                if (item != null)
                {
                    // Gán dữ liệu cho GetBookingDetailDTO
                    Room room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(item.RoomID);
                    GetBookingDetailDTO getBookingDetail = new GetBookingDetailDTO
                    {
                        RoomName = room.Name,
                    };
                    getBookingDTO.BookingDetail.Add(getBookingDetail);
                }
            }
            //Service
            foreach (var item in booking.ServiceBookings)
            {
                if (item != null)
                {
                    Service servier = await _unitOfWork.GetRepository<Service>().GetByIdAsync(item.ServiceID);
                    // Gán dữ liệu cho GetBookingDetailDTO
                    GetServiceBookingDTO getservice = new GetServiceBookingDTO
                    {
                        ServiceName = servier.Name,
                        Quantity=item.Quantity,
                    };
                    getBookingDTO.Services.Add(getservice);
                }
            }
            //Gửi mail về cho khách hàng xác nhận đã booking thành công 
            //Kèm theo thông tin của booking
            // Gửi mail về cho khách hàng xác nhận đã booking thành công 
            Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(booking.CustomerId);
            if (customer != null)
            {
                var emailService = new EmailService(_configuration, _unitOfWork); // Khởi tạo EmailService với logger
                await emailService.SendBookingConfirmationEmailAsync(booking, customer.Email, getBookingDTO);
            }


            return getBookingDTO;
        }


        //Duyệt-huỷ booking
        public async Task UpdateStatusBooking(string bookingID, EnumBooking enumBooking)
        {
            if (String.IsNullOrWhiteSpace(bookingID))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Mã booking không được để trống!");
            }
            Booking booking = await _unitOfWork.GetRepository<Booking>().GetByIdAsync(bookingID)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy booking!");

            booking.Status = enumBooking;
            if (booking.Status == EnumBooking.CONFIRMED)
            {
                //Gửi mail đã xác nhận 
                return;
            }
            //Yêu cầu huỷ từ khách hàng
            if (booking.Status == EnumBooking.CANCELLATIONREQUEST)
            {
                return;
            }
            //Nhân viên sẽ huỷ phòng cho khách hàng -- liên hệ trả cọc rồi huỷ
            if (booking.Status == EnumBooking.CANCELED)
            {
                return;
            }

            await _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task DeleteBookingAsync(string bookingID)
        {
            Booking bkDelete = await _unitOfWork.GetRepository<Booking>().GetByIdAsync(bookingID);

            if (bkDelete != null)
            {
                List<BookingDetail> listDetail = await _unitOfWork.GetRepository<BookingDetail>().Entities
                    .Where(bd => bd.BookingId == bkDelete.Id)
                    .ToListAsync();

                foreach (var bd in listDetail)
                {
                    await _unitOfWork.GetRepository<BookingDetail>().Entities
                        .Where(dt => dt.BookingId == bkDelete.Id && dt.RoomID == bd.RoomID)
                        .ExecuteDeleteAsync();
                    await _unitOfWork.SaveChangesAsync();
                }

                List<ServiceBooking> serviceBookings = await _unitOfWork.GetRepository<ServiceBooking>().Entities
                    .Where(sb => sb.BookingID == bookingID)
                    .ToListAsync();

                foreach (var servicebooking in serviceBookings)
                {
                    await _unitOfWork.GetRepository<ServiceBooking>().Entities
                        .Where(sb => sb.BookingID == bkDelete.Id && sb.ServiceID == servicebooking.ServiceID)
                        .ExecuteDeleteAsync();
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.GetRepository<Booking>().Entities
                    .Where(b => b.Id == bookingID)
                    .ExecuteDeleteAsync();

                await _unitOfWork.SaveChangesAsync();
            }
        }

    }
}

