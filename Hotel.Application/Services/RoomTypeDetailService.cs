
using AutoMapper;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Services
{
    public class RoomTypeDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomTypeDetailService> _logger;
        public RoomTypeDetailService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomTypeDetailService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<List<RoomTypeDetail>> GetAllRoomType()
        {
            return null;
        }
    }
}
