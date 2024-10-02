
using AutoMapper;
using Hotel.Application.DTOs.FloorDTO;
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Services
{
    public class FloorService : IFloorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FloorService> _logger;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public FloorService(IUnitOfWork unitOfWork, ILogger<FloorService> logger,IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task<List<GetFloorDTO>> GetAllFloor()
        {
            List<GetFloorDTO> floors = _mapper.Map<List<GetFloorDTO>>(await _unitOfWork.GetRepository<Floor>()
            .Entities.Where(r => r.DeletedTime == null).ToListAsync());
            return floors;
        }

        public async Task<GetFloorDTO> CreateFloor(PostFloorDTO postFloorDTO)
        {
            if (String.IsNullOrWhiteSpace(postFloorDTO.Name))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Không được để trống tên!");
            }
            Floor floor = await _unitOfWork.GetRepository<Floor>().Entities.FirstOrDefaultAsync(f => f.Name == postFloorDTO.Name)
                  ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.EXISTED, "Đã tồn tại tầng này!");
            
            Floor floorInsert =_mapper.Map<Floor>(postFloorDTO);

            string userID=Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            floorInsert.CreatedBy=floorInsert.LastUpdatedBy=userID;
            floorInsert.CreatedTime = floorInsert.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Floor>().InsertAsync(floor);
            await _unitOfWork.SaveChangesAsync();
            
            GetFloorDTO getFloorDTO = _mapper.Map<GetFloorDTO>(floorInsert);
            return getFloorDTO;
        }
    }
}
