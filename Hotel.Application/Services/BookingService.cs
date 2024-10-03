using AutoMapper;
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
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

        public async Task<List<GetBookingDTO>> GetAllBooking()
        {
            List<Booking> bookings = await _unitOfWork.GetRepository<Booking>().Entities.Where(b => b.DeletedTime == null).ToListAsync();
            List<GetBookingDTO> bookingDTOs=_mapper.Map<List<GetBookingDTO>>(bookings);
            if(bookingDTOs.Count == 0 )
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không có dữ liệu!");
            }
            return bookingDTOs;
        }
        public async Task<GetBookingDTO> CreateBooking(PostBookingDTO model)
        {
            return null;
        }
    }
}
