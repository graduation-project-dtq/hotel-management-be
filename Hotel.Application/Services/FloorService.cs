
using AutoMapper;
using Hotel.Application.DTOs.FloorDTO;
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.Interfaces;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Services
{
    public class FloorService : IFloorService
    {
        public IUnitOfWork _unitOfWork;
        public ILogger<FloorService> Logger;
        public IMapper _mapper;
        public FloorService(IUnitOfWork unitOfWork, ILogger<FloorService> logger,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            Logger = logger;
            _mapper = mapper;
        }

        public async Task<List<GetFloorDTO>> GetAllFloor()
        {
            List<GetFloorDTO> floors = _mapper.Map<List<GetFloorDTO>>(_unitOfWork.GetRepository<Floor>()
            .Entities.Where(r => r.DeletedTime == null).ToList());
            return floors;
        }
    }
}
