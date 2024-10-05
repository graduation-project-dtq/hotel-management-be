using AutoMapper;
using Hotel.Application.DTOs.HouseTypeDTO;
using Hotel.Application.DTOs.ServiceDTO;
using Hotel.Application.Interfaces;
using Hotel.Application.PaggingItems;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Services
{
    public class HouseTypeService : IHouseTypeService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private ILogger<HouseTypeService> _logger;
        public HouseTypeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HouseTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        //Lấy thông tin loại phòng
       public async Task<PaginatedList<GetHoustTypeDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<HouseType> query = _unitOfWork.GetRepository<HouseType>().Entities
                 .Include(r => r.Rooms)
                 .Where(c => !c.DeletedTime.HasValue)
                 .OrderByDescending(c => c.CreatedTime);
            // Tìm kiếm theo ID
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng với ID đã cho!");
                }
            }

            //Tìm theo tên
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
                return new PaginatedList<GetHoustTypeDTO>(new List<GetHoustTypeDTO>(), totalCount, index, pageSize);
            }
            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            List<GetHoustTypeDTO> responseItems = new List<GetHoustTypeDTO>();

            foreach (var item in resultQuery)
            {
                GetHoustTypeDTO dto = new GetHoustTypeDTO()
                {
                    Name = item.Name,
                    Description = item.Description,
                    Id = item.Id,
                };
                dto.Rooms = new List<GetRoomHouseTypeDTO>();

                if (item.Rooms != null)
                {
                    foreach (var room in item.Rooms)
                    {
                        GetRoomHouseTypeDTO roomHouseTypeDTO = new GetRoomHouseTypeDTO()
                        {
                            RoomName = room.Name
                        };
                        dto.Rooms.Add(roomHouseTypeDTO);
                    }
                }
                responseItems.Add(dto);
            }
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetHoustTypeDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }

        public async Task CreateHouseType(PostHouseTypeDTO model)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");
            if(!regex.IsMatch(model.Name))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Tên không hợp lệ");
            }
            if(!String.IsNullOrWhiteSpace(model.Description))
            {
                if (!regex.IsMatch(model.Description))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Mô tả không hợp lệ");
                }
            }    
            //Thêm vào csdl
            HouseType houseType=_mapper.Map<HouseType>(model);

            await _unitOfWork.GetRepository<HouseType>().InsertAsync(houseType);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
