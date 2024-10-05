
using AutoMapper;
using Hotel.Application.DTOs.FloorDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Common;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<GetFloorDTO> CreateFloor(PostFloorDTO model)
        {
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống tên!");
            }

            if (await _unitOfWork.GetRepository<Floor>().Entities.FirstOrDefaultAsync(f => f.Name == model.Name)!=null) 
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Đã tồn tại tầng này!");

            }

            Floor floorInsert =_mapper.Map<Floor>(model);

            string userID=Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            floorInsert.CreatedBy=floorInsert.LastUpdatedBy=userID;
            floorInsert.CreatedTime = floorInsert.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Floor>().InsertAsync(floorInsert);
            await _unitOfWork.SaveChangesAsync();
            
            GetFloorDTO getFloorDTO = _mapper.Map<GetFloorDTO>(floorInsert);
            return getFloorDTO;
        }
        public async Task<GetFloorDTO> UpdateFloor(string id, PutFloorDTO model)
       {
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Không được để trống tên!");
            }
            Floor floor = await _unitOfWork.GetRepository<Floor>().Entities.FirstOrDefaultAsync(f => f.Id == id)
                  ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.EXISTED, "Không tồn tại Floor với ID nhập vào!");

            floor = _mapper.Map<Floor>(model);

            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            floor.LastUpdatedBy = userID;
            floor.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Floor>().UpdateAsync(floor);
            await _unitOfWork.SaveChangesAsync();

            GetFloorDTO getFloorDTO = _mapper.Map<GetFloorDTO>(floor);
            return getFloorDTO;
        }
   
        public async Task DeleteFloor(string id)
       {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Không được để trống ID!");
            }
            Floor floor = await _unitOfWork.GetRepository<Floor>().Entities.FirstOrDefaultAsync(f => f.Id == id)
                  ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.EXISTED, "Đã tồn tại tầng này!");

            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            floor.LastUpdatedBy = userID;
            floor.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Floor>().InsertAsync(floor);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
