using AutoMapper;
using Hotel.Application.DTOs.RoomDTO;
using Hotel.Application.Extensions;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
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
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomService> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        public RoomService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomService> logger, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }
        public async Task<PaginatedList<GetRoomDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Room> query = _unitOfWork.GetRepository<Room>().Entities
                .Include(r=>r.Floor)
                .Include(r=>r.RoomTypeDetail)
                 .Where(c => !c.DeletedTime.HasValue)
                 .OrderByDescending(c => c.CreatedTime);

            // Áp dụng điều kiện tìm kiếm theo ID
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy phòng với ID đã cho!");
                }
            }

            // Áp dụng điều kiện tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                query = query.Where(r => r.Name.Contains(nameSearch));
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy phòng với tên đã cho!");
                }
            }

            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetRoomDTO>(new List<GetRoomDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            // Ánh xạ từ Room sang GetRoomDTO
            //var responseItems = resultQuery.Select(room => _mapper.Map<GetRoomDTO>(room)).ToList();
            List<GetRoomDTO> responseItems = new List<GetRoomDTO>();
            foreach (Room item in resultQuery)
            {
                Account account = await _unitOfWork.GetRepository<Account>().GetByIdAsync(item.CreatedBy);
                GetRoomDTO response = new GetRoomDTO()
                {
                    Id = item.Id,
                    FloorID = item.Floor != null ? item.Floor.Name : null,
                    RoomTypeDetailId = item.RoomTypeDetail != null ? item.RoomTypeDetail.Name : null,
                    Name=item.Name,
                    CreateBy=account !=null ? account.Name : item.CreatedBy,
                };
                responseItems.Add(response);
            }
            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetRoomDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }

        public async Task<List<GetRoomDTO>>GetAllRoom()
        {
            List<GetRoomDTO> query = _mapper.Map<List<GetRoomDTO>>(await _unitOfWork.GetRepository<Room>().Entities
                  .Where(r => r.DeletedTime == null)
                  .OrderByDescending(r => r.CreatedTime).ToListAsync());
            return query;
        }

        //Tìm theo id
        public async Task<GetRoomDTO> GetRoomById(string id)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");
            if (!regex.IsMatch(id.Trim()))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID không hợp lệ! Không được chứa ký tự đặc biệt.");
            }
            var room = await _unitOfWork.GetRepository<Room>().Entities.FirstOrDefaultAsync(r=>r.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy");

            GetRoomDTO getRoomDTO=_mapper.Map<GetRoomDTO>(room);

            return getRoomDTO;
        }
    
        public async Task<GetRoomDTO> CreateRoom(PostRoomDTO model)
        {
           
            if (String.IsNullOrWhiteSpace(model.RoomTypeDetailId))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Không được để trống loại phòng!");
            }

            //Check khoá ngoại
            var roomTypeDetail = await _unitOfWork.GetRepository<RoomTypeDetail>().Entities.FirstOrDefaultAsync(r => r.Id == model.RoomTypeDetailId);
            if (roomTypeDetail == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng!");
            }

           
          

            if (String.IsNullOrWhiteSpace(model.FloorID))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.EXISTED, "Không được để trống vị trí!");
            }

            var floor = await _unitOfWork.GetRepository<Floor>().Entities.FirstOrDefaultAsync(f => f.Id == model.FloorID);
            if (floor == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy vị trí!");
            }
            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            Room room=_mapper.Map<Room>(model);
            room.CreatedBy = userId;
            room.LastUpdatedBy = userId;
            room.CreatedTime=room.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<Room>().InsertAsync(room);
            await _unitOfWork.SaveChangesAsync();

            GetRoomDTO getRoomDTO=_mapper.Map<GetRoomDTO>(room);
            return getRoomDTO;
        }

        //Tìm room khi booking 
        public async Task<List<GetRoomDTO>> FindRoomBooking(FindRoomDTO model)
        {
            if(model.CheckInDate>=model.CheckOutDate)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Ngày đến phải nhỏ hơn ngày đi");
            }
        
            List<Room> rooms = await _unitOfWork.GetRepository<Room>().Entities
                .Where(r => r.DeletedTime == null && r.IsActive == true && r.RoomTypeDetailId == model.RoomTypeDetailID)
                .ToListAsync();

            var bookedRooms = await _unitOfWork.GetRepository<BookingDetail>().Entities
                .Where(bd => bd.Room.DeletedTime == null &&
                             bd.Room.IsActive == true &&
                             bd.Room.RoomTypeDetailId == model.RoomTypeDetailID &&
                             (bd.Booking.CheckInDate < model.CheckOutDate && bd.Booking.CheckOutDate > model.CheckInDate))
                .Select(bd => bd.RoomID)
                .ToListAsync();

            var availableRooms = rooms.Where(r => !bookedRooms.Contains(r.Id)).ToList();
            var roomDTOs = _mapper.Map<List<GetRoomDTO>>(availableRooms.Select(r => new GetRoomDTO
            {
                Id = r.Id,
                Name = r.Name,
            }).ToList());
            if (roomDTOs.Count == 0)
            {
                return null;
            }
            return roomDTOs;
        }


    }
}
