
using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
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
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomTypeService> _logger;
        public RoomTypeService(IUnitOfWork unitOfWork,IMapper mapper ,ILogger<RoomTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<List<GetRoomTypeDTO>> GetAllRoomType()
        {
            List<GetRoomTypeDTO> roomTypes= _mapper.Map<List<GetRoomTypeDTO>>(_unitOfWork.GetRepository<RoomType>()
                .Entities.Where(r => r.DeletedTime == null).ToList());
            //Get all roomtype 
            return roomTypes;
        }
        public async Task<RoomType> CreateRoomType(CreateRoomTypeDTO model)
        {
            try
            {
                RoomType? exitRoomType = await _unitOfWork.GetRepository<RoomType>()
                .Entities.FirstOrDefaultAsync(r => r.Name == model.Name);
                if (exitRoomType != null)
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "RoomType is existed!");
                }
                RoomType roomType = _mapper.Map<RoomType>(model);
                await _unitOfWork.GetRepository<RoomType>().InsertAsync(roomType);
                await _unitOfWork.SaveChangesAsync();
                return roomType;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString()+ "\tInsert RoomType faled");
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.EXISTED, "Insert RoomType faled!");
            }
        }

        public async Task DeleteRoomType(string id)
        {

        }
    }
}
