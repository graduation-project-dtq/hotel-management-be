using AutoMapper;
using Hotel.Application.DTOs.BookingDTO;
using Hotel.Application.Interfaces;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return null;
        }
        public async Task<GetBookingDTO> CreateBooking(PostBookingDTO model)
        {
            return null;
        }
    }
}
