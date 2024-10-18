using AutoMapper;
using Hotel.Application.DTOs.BookingDetailDTO;
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.DTOs.NotificationDTO;
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
        private readonly INotificationService _notificationService;
        public BookingService(IUnitOfWork unitOfWork, ILogger<BookingService> logger, IMapper mapper,
            IHttpContextAccessor contextAccessor, IRoomService roomService, IEmailService emailService
            , IConfiguration configuration, IRoomTypeDetailService roomTypeDetailService
            , INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _roomService = roomService;
            _emailService = emailService;
            _configuration = configuration;
            _roomTypeDetailService = roomTypeDetailService;
            _notificationService = notificationService;
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
            //Lấy danh sách theo điều kiện
            List<Booking> bookings = await _unitOfWork.GetRepository<Booking>().Entities
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                .Include(b => b.ServiceBookings)
                .Include(b=>b.Punishes)
                .Where(b => b.CustomerId == customerId && b.Status == enumBooking && b.DeletedTime == null).ToListAsync();
            //Mapping 
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
                bookingModel.BookingDate = item.CreatedTime.Date.ToString("dd/MM/yyyy HH:mm:ss");
                bookingModel.TotalAmount = item.TotalAmount;
                bookingModel.UnpaidAmount = item.UnpaidAmount;
                bookingModel.CheckInDate = item.CheckInDate.ToString("dd/MM/yyyy");
                bookingModel.CheckOutDate = item.CheckOutDate.ToString("dd/MM/yyyy");
                bookingModel.BookingDetail = new List<GetBookingDetailDTO>();
                bookingModel.Services = new List<GetServiceBookingDTO>();
                bookingModel.Punishes = new List<GetPunishesDTO>();
                //Thêm chi tiết phòng
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
                //Thêm dịch vụ
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
                //Thêm tiền phạt nếu có
                if(item.Punishes.Count>0)
                {
                    foreach(Punish punish in item.Punishes)
                    {
                        GetPunishesDTO punishesDTO = new GetPunishesDTO()
                        {
                            FacilitiesName = punish.Facilities.Name,
                            Quantity=punish.Quantity,
                            Fine=punish.Fine,
                        };
                        item.Punishes.Add(punish);
                    }    
                }    
                getBookingDTO.Add(bookingModel);
            }
            return getBookingDTO;
        }
        public async Task<GetBookingDTO> CreateBooking(PostBookingDTO model)
        {
            //get Id từ author
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
            //Kiểm tra có sử dụng voucher hay không
            //Nếu có kiểm tra tính khả dụng của voucher
            Voucher voucher = new Voucher()
            {
                Id = null,
                DiscountAmount = 0
            };
            if (!String.IsNullOrWhiteSpace(model.VoucherId))
            {
                voucher = await _unitOfWork.GetRepository<Voucher>().Entities
                   .Where(v => v.Id == model.VoucherId && v.DeletedTime == null
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
                PromotionalPrice = voucher.DiscountAmount,
                VoucherId = voucher.Id,
                //PromotionalPrice = 0,
                //VoucherId = null,
                Customer = null,
                IdentityCard = null,
            };

            await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            string bookingID = booking.Id;
            //Thêm phòng
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
                    //Rollback
                    if (listRoomActive == null )
                    {
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
                    //Rollback
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

           
          
            //Trừ số lượng voucher
            voucher.Quantity = voucher.Quantity - 1;
            booking.TotalAmount = booking.TotalAmount - booking.PromotionalPrice;
            if (booking.Deposit > 0)
            {
                booking.UnpaidAmount = booking.TotalAmount - booking.Deposit;
            }
           
            if (voucher.Quantity == 0)
            {
                voucher.IsActive = false; //Đã sử dụng hết voucher
                await _unitOfWork.GetRepository<Voucher>().UpdateAsync(voucher);
                await _unitOfWork.SaveChangesAsync();
            }
            await _unitOfWork.SaveChangesAsync();

            //Tạo thông báo
            PostNotificationDTO notificationDTO = new PostNotificationDTO()
            {
                CustomerId = booking.CustomerId,
                Title = "Đơn đặt phòng thành công",
                Content = "Đã đặt phòng thành công , vui lòng chờ khách sạn chúng tôi xác nhận.Xin cảm ơn quý khách"
            };

            await _notificationService.CreateNotification(notificationDTO);
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
        public async Task UpdateStatusBooking(string bookingID)
        {
            if (String.IsNullOrWhiteSpace(bookingID))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Mã booking không được để trống!");
            }
            Booking booking = await _unitOfWork.GetRepository<Booking>().GetByIdAsync(bookingID)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy booking!");
            PostNotificationDTO notificationDTO = new PostNotificationDTO()
            {
                CustomerId = booking.CustomerId,
            };
            //Khách hàng yêu cầu huỷ
            if (booking.Status == EnumBooking.CANCELLATIONREQUEST)
            {
                booking.Status = EnumBooking.CANCELED;
                //Kiểm tra nếu ngày huỷ và ngày CheckIn > 2 thì sẽ trả lại tiền cọc <=2 thì sẽ không được trả lại tiền
                
                int count = (booking.CheckInDate.DayNumber - DateOnly.FromDateTime(CoreHelper.SystemTime).DayNumber);
                if(count >2)
                {
                    //Gửi mail liên hệ hoàn tiền
                    await _emailService.SendEmailAsync(booking.Customer.Email, false, new GetBookingDTO { Id = booking.Id, Deposit = booking.Deposit },count);
                }    

                else
                {
                    //Gửi mail thông báo không được hoàn tiền
                    await _emailService.SendEmailAsync(booking.Customer.Email, false, new GetBookingDTO { Id = booking.Id, Deposit = booking.Deposit }, count);
                }
                notificationDTO.Title = "Thông tin huỷ phòng";
                notificationDTO.Content = "Yêu cầu huỷ đặt phòng có mã " + booking.Id + " đã được huỷ thành công";
            }
            //Xác nhận đặt phòng
            if (booking.Status == EnumBooking.UNCONFIRMED)
            {
                booking.Status = EnumBooking.CANCELED;
                //Gửi mail đã xác nhận 
                // Gửi mail đã xác nhận đặt phòng
                var bookingDTO = new GetBookingDTO
                {
                    Id = booking.Id,
                    CustomerName = booking.Customer.Name,
                    PhoneNumber = booking.PhoneNumber,
                    CheckInDate =booking.CheckInDate.ToString("dd/MM/yyyy"),
                    CheckOutDate = booking.CheckOutDate.ToString("dd/MM/yyyy"),
                    TotalAmount = booking.TotalAmount,
                    PromotionalPrice = booking.PromotionalPrice,
                    Deposit = booking.Deposit
                };
                await _emailService.SendEmailAsync(booking.Customer.Email, true, bookingDTO, 0);
                notificationDTO.Title = "Xác nhân phòng";
                notificationDTO.Content = "Đặt phòng có mã " + booking.Id + " đã được xác khách sạn xác nhận thành công thành công";
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
        //CheckIn
       public async Task CheckIn(CheckInDTO model)
       {
            Booking booking = await _unitOfWork.GetRepository<Booking>().Entities
                .Where(bk => bk.Id.Equals(model.BookingId) && bk.Status == EnumBooking.CONFIRMED) //Đã xác nhận
                .FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy lịch đặt phòng");

            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            booking.Status = EnumBooking.CHECKEDIN;
            booking.CustomerName = model.CustomerName;
            booking.IdentityCard = model.IdentityCard;
            booking.LastUpdatedBy = userId;
            booking.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
       }

        //Checkout
        public async Task CheckOut(CheckOutDTO model)
        {
            Booking booking = await _unitOfWork.GetRepository<Booking>().Entities
                .Where(bk=>bk.Id == model.BookingId && bk.DeletedTime == null && bk.Status == EnumBooking.CHECKEDIN)
                .FirstOrDefaultAsync()
               ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy lịch đặt phòng");
            
            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            //Tính tiền phạt
            if(model.Punishes != null && model.Punishes.Count>0)
            {
                booking.Punishes = new List<Punish>();
                decimal pricePunish = 0;
                foreach(PostPunishesDTO item in model.Punishes)
                {
                    Facilities facilities = await _unitOfWork.GetRepository<Facilities>().GetByIdAsync(item.FacilitiesID)
                        ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Đồ dùng, tài sản không hợp lệ!");

                    Punish punish = new Punish()
                    {
                        BookingID = booking.Id,
                        FacilitiesID = item.FacilitiesID,
                        Quantity = item.Quantity,
                        Fine = item.Quantity * facilities.Price
                    };

                    booking.Punishes.Add(punish);
                    booking.TotalAmount += punish.Fine;
                    pricePunish += punish.Fine;
                }
                //Cập nhật lại tiền cần thanh toán
                booking.UnpaidAmount = booking.TotalAmount - pricePunish;
                booking.LastUpdatedBy = userId;
                booking.LastUpdatedTime = CoreHelper.SystemTimeNow;

                //Cập nhật sau khi checkout thành công 
                //Cập nhật lại số tiền đã thanh toán là tổng tiền hoá đơn và tiền chưa thanh toán bằng 0

                //booking.UnpaidAmount = 0;

                //Lưu kết quả
                await _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                await _unitOfWork.SaveChangesAsync();

            }
            //Gửi email cảm ơn kèm hoá đơn
            //Tạo report để gửi cho khách hàng
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
                Services = new List<GetServiceBookingDTO>(),
                Punishes = new List<GetPunishesDTO>()
            };

            // Thêm thông tin BookingDetail vào DTO
            if (booking.BookingDetails.Count > 0)
            {
                foreach (var item in booking.BookingDetails)
                {
                    // Gán dữ liệu cho GetBookingDetailDTO
                    //Room room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(item.RoomID);
                    GetBookingDetailDTO getBookingDetail = new GetBookingDetailDTO
                    {
                        RoomName = item.Room.Name
                    };
                    getBookingDTO.BookingDetail.Add(getBookingDetail);
                }
            }


            //Service
            if (booking.ServiceBookings.Count > 0)
            {
                foreach (var item in booking.ServiceBookings)
                {
                    //Service servier = await _unitOfWork.GetRepository<Service>().GetByIdAsync(item.ServiceID);
                    // Gán dữ liệu cho GetBookingDetailDTO
                    GetServiceBookingDTO getservice = new GetServiceBookingDTO
                    {
                        ServiceName = item.Service.Name,
                        Quantity = item.Quantity,
                    };
                    getBookingDTO.Services.Add(getservice);
                }
            }
            if (booking.Punishes.Count > 0)
            {
                foreach (var item in booking.Punishes)
                {

                    GetPunishesDTO punishesDTO = new GetPunishesDTO
                    {
                        FacilitiesName = item.Facilities.Name,
                        Quantity = item.Quantity,
                        Fine = item.Fine,
                    };
                    getBookingDTO.Punishes.Add(punishesDTO);
                }
            }
            //Gửi mail về cho khách hàng xác nhận đã booking thành công 
            //Kèm theo thông tin của booking
            // Gửi mail về cho khách hàng xác nhận đã booking thành công 
            Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(booking.CustomerId);
            if (customer != null)
            {
                var emailService = new EmailService(_configuration, _unitOfWork); // Khởi tạo EmailService với logger
                try
                {
                    await emailService.SendBookingConfirmationEmailAsync(booking, customer.Email, getBookingDTO);
                    _logger.LogInformation("Gửi mail thành công!");
                }
                catch
                {
                    _logger.LogError("Gửi mail thất bại!");
                }
            }
        }
        public async Task HuyPhong(string id)
        {
            String userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            if(String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập mã booking!");
            }
            Booking booking  = await _unitOfWork.GetRepository<Booking>().GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Mã booking không hợp lệ!");

            //Update status huỷ phòng 
            booking.Status = EnumBooking.CANCELLATIONREQUEST;
            booking.LastUpdatedBy = userID;
            booking.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            //Tạo thông báo huỷ phòng
            PostNotificationDTO notificationDTO = new PostNotificationDTO()
            {
                CustomerId = booking.CustomerId,
                Title="Yêu cầu huỷ phòng",
                Content="Bạn đã yêu cầu huỷ đặc phòng cho mã đặc phòng "+ booking.Id+" thành công",
            };
            await _notificationService.CreateNotification(notificationDTO);
        }
    }
}

