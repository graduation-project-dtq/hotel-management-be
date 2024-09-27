
using AutoMapper;
using Hotel.Application.DTOs.RoomTypeDetailDTO;
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
    public class RoomTypeDetailService //: IRoomTypeDetailService
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
        public async Task<List<GetRoomTypeDetailDTO>> GetAllRoomTypeDetail()
        {
            List<GetRoomTypeDetailDTO> roomTypeDetails = _mapper.Map<List<GetRoomTypeDetailDTO>>(_unitOfWork.GetRepository<RoomTypeDetail>()
              .Entities.Where(r => r.DeletedTime == null).ToList());
            return roomTypeDetails;
        }

        //public async Task<PortRoomTypeDetailDTO> CreateRoomTypeDetail(PortRoomTypeDetailDTO portRoomTypeDetail)
        //{
        //    var roomtype = await _unitOfWork.GetRepository<RoomType>().Entities.FirstOrDefaultAsync(r => r.Id == portRoomTypeDetail.RoomTypeID);
        //    if (roomtype == null)
        //    {
        //        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.EXISTED, "Không tồn tại loại phòng!");
        //    }
        //    //Kiểm tra tên
        //    var roomTypeDetailExit = await _unitOfWork.GetRepository<RoomTypeDetail>().Entities.FirstOrDefaultAsync(r=>r.Name == portRoomTypeDetail.Name);
        //    if(roomTypeDetailExit != null)
        //    {
        //        throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.DUPLICATE, "Chi tiết loại phòng đã tồn tại!");
        //    }

        //}
    }
}
