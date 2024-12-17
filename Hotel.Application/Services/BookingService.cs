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

        public async Task<PaginatedList<GetBookingDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID
            , string customerName, DateOnly? bookingDate, DateOnly? checkInDate, EnumBooking ? status, string phone)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Booking> query = _unitOfWork.GetRepository<Booking>().Entities
                 .Include(c => c.Customer!)
                 .Include(bk => bk.BookingDetails!)
                    .ThenInclude(bd => bd.Room!)
                 .Include(bk => bk.ServiceBookings!)
                    .ThenInclude(sv=>sv.Service!)
                 .Include(bk=>bk.Punishes!)
                    .ThenInclude(p=>p.Facilities)
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
                query = query.Where(r => r.Customer!=null && r.Customer.Name.ToString().Contains(customerName));
            }
            //Tìm theo ngày đặt phòng
            if (bookingDate != null)
            {
                query = query.Where(bk => DateOnly.FromDateTime(bk.CreatedTime.Date) == bookingDate);
            }
            //Tìm theo ngày checkin
            if (checkInDate != null)
            {
                query = query.Where(bk => bk.CheckInDate == checkInDate);
            }
            //Tìm theo status
            if(status!= null)
            {
                query = query.Where(bk => bk.Status == status);
            }
            //Tìm theo phone
            if(!string.IsNullOrWhiteSpace(phone))
            {
                query = query.Where(bk => bk.PhoneNumber.Equals(phone));
            }
            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetBookingDTO>(new List<GetBookingDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();


            List<GetBookingDTO> responseItems = new List<GetBookingDTO>();
            responseItems = query.Select(bk => new GetBookingDTO()
            {
                Id = bk.Id,
                EmployeeId = bk.EmployeeId,
                CustomerId = bk.CustomerId,
                CustomerName = bk.CustomerName,
                PhoneNumber = bk.PhoneNumber,
                PromotionalPrice = bk.PromotionalPrice,
                Deposit = bk.Deposit,
                BookingDate = bk.CreatedTime.Date.ToString("dd/MM/yyyy HH:mm:ss"),
                TotalAmount = bk.TotalAmount,
                UnpaidAmount = bk.UnpaidAmount,
                CheckInDate = bk.CheckInDate.ToString("dd/MM/yyyy"),
                CheckOutDate = bk.CheckOutDate.ToString("dd/MM/yyyy"),
                BookingDetail = bk.BookingDetails != null ? bk.BookingDetails.Select(bd => new GetBookingDetailDTO()
                {
                    RoomName = bd.Room != null ? bd.Room.Name : string.Empty,
                }).ToList() : new List<GetBookingDetailDTO>(),
                Services = bk.ServiceBookings != null ? bk.ServiceBookings.Select(sv => new GetServiceBookingDTO()
                {
                    ServiceName = sv.Service != null ? sv.Service.Name : string.Empty,
                    Quantity = sv.Quantity,
                }).ToList() : new List<GetServiceBookingDTO>(),
                Punishes = bk.Punishes != null ? bk.Punishes.Select(p => new GetPunishesDTO()
                {
                    FacilitiesName = p.Facilities != null ? p.Facilities.Name : string.Empty,
                    Quantity = p.Quantity,
                    Fine = p.Fine,
                }).ToList() : new List<GetPunishesDTO>(),
            }).ToList();

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
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID khách hàng");
            }
            //Lấy danh sách theo điều kiện
            List<Booking> bookings = await _unitOfWork.GetRepository<Booking>().Entities
                .Include(c => c.Customer!)
                 .Include(bk => bk.BookingDetails!)
                    .ThenInclude(bd => bd.Room!)
                 .Include(bk => bk.ServiceBookings!)
                    .ThenInclude(sv => sv.Service!)
                 .Include(bk => bk.Punishes!)
                    .ThenInclude(p => p.Facilities)
                .Where(b => b.CustomerId == customerId && b.Status == enumBooking && !b.DeletedTime.HasValue).ToListAsync();
            //Mapping 
            List<GetBookingDTO> getBookingDTO = new List<GetBookingDTO>();

            getBookingDTO = bookings.Select(bk => new GetBookingDTO()
            {
                Id = bk.Id,
                EmployeeId = bk.EmployeeId,
                CustomerId = bk.CustomerId,
                CustomerName = bk.CustomerName,
                PhoneNumber = bk.PhoneNumber,
                PromotionalPrice = bk.PromotionalPrice,
                Deposit = bk.Deposit,
                BookingDate = bk.CreatedTime.Date.ToString("dd/MM/yyyy HH:mm:ss"),
                TotalAmount = bk.TotalAmount,
                UnpaidAmount = bk.UnpaidAmount,
                CheckInDate = bk.CheckInDate.ToString("dd/MM/yyyy"),
                CheckOutDate = bk.CheckOutDate.ToString("dd/MM/yyyy"),
                BookingDetail = bk.BookingDetails != null ? bk.BookingDetails.Select(bd => new GetBookingDetailDTO()
                {
                    RoomName = bd.Room != null ? bd.Room.Name : string.Empty,
                }).ToList() : new List<GetBookingDetailDTO>(),
                Services = bk.ServiceBookings != null ? bk.ServiceBookings.Select(sv => new GetServiceBookingDTO()
                {
                    ServiceName = sv.Service != null ? sv.Service.Name : string.Empty,
                    Quantity = sv.Quantity,
                }).ToList() : new List<GetServiceBookingDTO>(),
                Punishes = bk.Punishes != null ? bk.Punishes.Select(p => new GetPunishesDTO()
                {
                    FacilitiesName = p.Facilities != null ? p.Facilities.Name : string.Empty,
                    Quantity = p.Quantity,
                    Fine = p.Fine,
                }).ToList() : new List<GetPunishesDTO>(),
            }).ToList();

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
            if (!string.IsNullOrWhiteSpace(model.VoucherId))
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
                //Tiền
                Deposit = model.Deposit,
                PromotionalPrice = voucher.DiscountAmount,
                TotalAmount = 0,
                DiscountedAmount=0,
                UnpaidAmount = 0,
                PricePunish=0,
                //------------
                PhoneNumber = model.PhoneNumber,
                CheckInDate = model.CheckInDate,
                CheckOutDate = model.CheckOutDate,
                BookingDetails = new List<BookingDetail>(),
                ServiceBookings = new List<ServiceBooking>(),
                
                VoucherId = voucher.Id,
                Customer = null,
                IdentityCard = null,
            };
            if(!string.IsNullOrWhiteSpace(model.EmployeeId))
            {
                Employee employee = await _unitOfWork.GetRepository<Employee>().Entities.Where(e => e.Id.Equals(model.EmployeeId)).FirstOrDefaultAsync()
                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Nhân viên không tồn tại");
                
                booking.EmployeeId = model.EmployeeId;
            }
            await _unitOfWork.GetRepository<Booking>().InsertAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            string bookingID = booking.Id;
            //Thêm phòng
            if (model.BookingDetails != null )
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

                    string ? roomID = listRoomActive != null ? listRoomActive[0].Id : string.Empty;

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
                    decimal price = await _roomTypeDetailService.GetDiscountPrice(item.RoomTypeDetailID);
                    int dayhere = model.CheckOutDate.DayNumber - model.CheckInDate.DayNumber;
                    _logger.LogError(dayhere.ToString());
                    if (price != 0)
                    {
                        booking.TotalAmount = booking.TotalAmount + (price * dayhere);
                    }
                    else
                    {
                        RoomTypeDetail ? roomTypeDetail= await _unitOfWork.GetRepository<RoomTypeDetail>()
                            .Entities
                            .Where(r=>r.Id.Equals(item.RoomTypeDetailID))
                            .FirstOrDefaultAsync();
                        if(roomTypeDetail!= null)
                        {
                            booking.TotalAmount += (roomTypeDetail.BasePrice * dayhere);
                        }
                    }
                }
            }
            //Thêm dịch vụ
            if (model.Services != null )
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

                    ServiceBooking serviceBooking = new ServiceBooking()
                    {
                        BookingID = booking.Id,
                        ServiceID = item.ServiceID,
                        Quantity = item.Quantity,
                    };

                    await _unitOfWork.GetRepository<ServiceBooking>().InsertAsync(serviceBooking);

                    Service? service = await _unitOfWork.GetRepository<Service>()
                        .Entities
                        .Where(s => s.Id.Equals(item.ServiceID))
                        .FirstOrDefaultAsync();
                    //Tính tiền dịch vụ
                    booking.TotalAmount +=  service.Price  * item.Quantity;
                }
            }

            //Trừ số lượng voucher
            voucher.Quantity = voucher.Quantity - 1;
            if(booking.PromotionalPrice >= booking.TotalAmount)
            {
                booking.DiscountedAmount = 0;
                booking.UnpaidAmount = 0;
            }    
            booking.DiscountedAmount = booking.TotalAmount - booking.PromotionalPrice;
            if (booking.Deposit > 0)
            {
                if(booking.Deposit == booking.TotalAmount)
                {
                    booking.UnpaidAmount = 0;
                }    
                else
                {
                    booking.UnpaidAmount = booking.DiscountedAmount - booking.Deposit;
                }
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
                DiscountedAmount=booking.DiscountedAmount,
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
                    Room room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(item.RoomID != null ? item.RoomID : string.Empty);
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
              
                if(!string.IsNullOrWhiteSpace(customer.Email))
                {
                    await _emailService.SendBookingConfirmationEmailAsync(booking, customer.Email, getBookingDTO);
                }
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
            Booking booking = await _unitOfWork.GetRepository<Booking>().Entities
                .Include(bk=>bk.Customer)
                .FirstOrDefaultAsync(bk=>bk.Id.Equals(bookingID) && !bk.DeletedTime.HasValue)
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
                
                int count = (booking.CheckInDate.DayNumber - CoreHelper.SystemDateOnly.DayNumber);
                if(count >2)
                {
                    //Gửi mail liên hệ hoàn tiền
                    if(booking.Customer != null && ! string.IsNullOrWhiteSpace(booking.Customer.Email))
                    {
                        await _emailService.SendEmailAsync(booking.Customer != null ? booking.Customer.Email : "", false, new GetBookingDTO { Id = booking.Id, Deposit = booking.Deposit }, count);
                    }
                }    

                else
                {
                    //Gửi mail thông báo không được hoàn tiền
                    if (booking.Customer != null && !string.IsNullOrWhiteSpace(booking.Customer.Email))
                    {
                        await _emailService.SendEmailAsync(booking.Customer != null ? booking.Customer.Email : "", false, new GetBookingDTO { Id = booking.Id, Deposit = booking.Deposit }, count);
                    }
                }
                notificationDTO.Title = "Thông tin huỷ phòng";
                notificationDTO.Content = "Yêu cầu huỷ đặt phòng có mã " + booking.Id + " đã được huỷ thành công";
            }
            //Xác nhận đặt phòng
            if (booking.Status == EnumBooking.UNCONFIRMED)
            {
                booking.Status = EnumBooking.CONFIRMED;
                //Gửi mail đã xác nhận 
                // Gửi mail đã xác nhận đặt phòng
                GetBookingDTO bookingDTO = _mapper.Map<GetBookingDTO>(booking);
                if (booking.Customer != null && !string.IsNullOrWhiteSpace(booking.Customer.Email))
                {
                    await _emailService.SendEmailAsync(booking.Customer != null ? booking.Customer.Email : "", true, bookingDTO, 0);
                }
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
            booking.CustomerCount = model.CustomerCount;
            booking.LastUpdatedBy = userId;
            booking.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();
       }

        //Checkout
        public async Task CheckOut(CheckOutDTO model)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        Booking booking = await _unitOfWork.GetRepository<Booking>().Entities
                            .Where(bk => bk.Id == model.BookingId && bk.DeletedTime == null && bk.Status == EnumBooking.CHECKEDIN)
                            .FirstOrDefaultAsync()
                           ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy lịch đặt phòng");

                        string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
                        decimal pricePunish = 0;
                        booking.Punishes = model.Punishes != null
                          ? (await Task.WhenAll(model.Punishes.Select(async p =>
                          {
                              Facilities ? facility = await _unitOfWork.GetRepository<Facilities>()
                                  .Entities.FirstOrDefaultAsync(f => f.Id.Equals(p.FacilitiesID) && !f.DeletedTime.HasValue);

                              if (facility == null)
                              {
                                  await _unitOfWork.RollBackAsync();
                                  throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy nội thất");
                              }

                              Punish punish =  new Punish()
                              {
                                  BookingID = model.BookingId,
                                  FacilitiesID = p.FacilitiesID,
                                  Note = p.Note,
                                  Fine = p.Quantity * facility.Price,
                              };

                              await _unitOfWork.GetRepository<Punish>().InsertAsync(punish);
                              await _unitOfWork.SaveChangesAsync();
                              return punish;

                          }))).ToList()
                          : new List<Punish>();

                        booking.UnpaidAmount = booking.TotalAmount - pricePunish;
                        booking.LastUpdatedBy = userId;
                        booking.LastUpdatedTime = CoreHelper.SystemTimeNow;

                       
                        //Gửi email cảm ơn kèm hoá đơn
                        //Tạo report để gửi cho khách hàng
                        GetBookingDTO getBookingDTO = new GetBookingDTO
                        {
                            Id = booking.Id,
                            EmployeeId = booking.EmployeeId,
                            CustomerId = booking.CustomerId,
                            CustomerName = booking.CustomerName,
                            PhoneNumber = booking.PhoneNumber,
                            PromotionalPrice = booking.PromotionalPrice,
                            Deposit = booking.Deposit,
                            BookingDate = booking.CreatedTime.Date.ToString("dd/MM/yyyy HH:mm:ss"),
                            TotalAmount = booking.TotalAmount,
                            UnpaidAmount = booking.UnpaidAmount,
                            CheckInDate = booking.CheckInDate.ToString("dd/MM/yyyy"),
                            CheckOutDate = booking.CheckOutDate.ToString("dd/MM/yyyy"),
                            BookingDetail = booking.BookingDetails != null ? booking.BookingDetails.Select(bkd => new GetBookingDetailDTO()
                            {
                                RoomName = bkd.Room != null ? bkd.Room.Name : string.Empty

                            }).ToList() : new List<GetBookingDetailDTO>(),

                            Services = booking.ServiceBookings != null ? booking.ServiceBookings.Select(sv => new GetServiceBookingDTO()
                            {
                                ServiceName = sv.Service != null ? sv.Service.Name : string.Empty,
                                Quantity = sv.Quantity,

                            }).ToList() : new List<GetServiceBookingDTO>(),
                            Punishes = booking.Punishes != null ? booking.Punishes.Select(p => new GetPunishesDTO()
                            {
                                FacilitiesName = p.Facilities != null ? p.Facilities.Name : string.Empty,
                                Quantity = p.Quantity,
                                Fine = p.Fine,

                            }).ToList() : new List<GetPunishesDTO>(),
                        };

                        //Gửi mail về cho khách hàng xác nhận đã booking thành công 
                        //Kèm theo thông tin của booking
                        // Gửi mail về cho khách hàng xác nhận đã booking thành công 
                        Customer customer = await _unitOfWork.GetRepository<Customer>().GetByIdAsync(booking.CustomerId);
                        if (customer.Email != null)
                        {
                            try
                            {
                                await _emailService.SendBookingConfirmationEmailAsync(booking, customer.Email, getBookingDTO);
                                _logger.LogInformation("Gửi mail thành công!");
                            }
                            catch
                            {
                                _logger.LogError("Gửi mail thất bại!");
                            }
                        }
                        //Update thêm điểm cho khách hàng
                        customer.AccumulatedPoints += (int)(booking.DiscountedAmount / 10000); //Cộng thêm bao nhiêu điểm
                        booking.Status = EnumBooking.CHECKEDOUT;
                        await _unitOfWork.GetRepository<Booking>().UpdateAsync(booking);
                        await _unitOfWork.GetRepository<Customer>().UpdateAsync(customer);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitTransactionAsync();
                    }
                    catch
                    {
                        await _unitOfWork.RollBackAsync();
                        throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Đã xảy ra lỗi");
                    }
                }
            });
            
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

        //Thống kê booking theo ngày
        public async Task<StatisticaInDTO> StatisticaInDate(DateOnly date)
        {
            List<Booking> bookings = await _unitOfWork.GetRepository<Booking>()
                .Entities
                .Where(b => !b.DeletedTime.HasValue && DateOnly.FromDateTime(b.CreatedTime.Date) ==date)
                .ToListAsync();
            StatisticaInDTO statisticaInDTO = new StatisticaInDTO()
            {
                Count = bookings != null ? bookings.Count : 0,
                TotalAmount = bookings !=null ? bookings.Select(b => b.TotalAmount).Sum() : 0,
                CustomerCount = bookings != null ? bookings.Select(b => b.CustomerCount ?? 0).Sum() : 0,

            }; ;
            return statisticaInDTO;
        }
        //Thông kê theo tháng
        public async Task<StatisticaInDTO> StatisticaInMonth(int month, int year)
        {
            if(month<1 || month>12)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Tháng không hợp lệ");
            }
            if(year <0 || year>DateTime.Now.Year)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Năm không hợp lệ");

            }
            List<Booking> bookings = await _unitOfWork.GetRepository<Booking>()
                .Entities
                .Where(b => !b.DeletedTime.HasValue &&
                            b.CreatedTime.Month == month &&
                            b.CreatedTime.Year == year)
                .ToListAsync();

            StatisticaInDTO statisticaInDTO = new StatisticaInDTO()
            {
                Count = bookings.Count,
                TotalAmount = bookings.Select(b => b.TotalAmount).Sum(),
                CustomerCount = bookings != null ? bookings.Select(b => b.CustomerCount ?? 0).Sum() : 0,
            };

            return statisticaInDTO;
        }
        //Thông kê theo năm
        public async Task<StatisticaInDTO> StatisticaInYear( int year)
        {
            if (year < 0 || year > DateTime.Now.Year)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Năm không hợp lệ");

            }
            List<Booking> bookings = await _unitOfWork.GetRepository<Booking>()
                .Entities
                .Where(b => !b.DeletedTime.HasValue &&
                            b.CreatedTime.Year == year)
                .ToListAsync();


            StatisticaInDTO statisticaInDTO = new StatisticaInDTO()
            {
                Count = bookings.Count,
                TotalAmount = bookings.Select(b => b.TotalAmount).Sum(),
                CustomerCount = bookings != null ? bookings.Select(b => b.CustomerCount ?? 0).Sum() : 0,
            };

            return statisticaInDTO;
        }

    }
}

