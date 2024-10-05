
using AutoMapper;
using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
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
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomTypeService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoomTypeService(IUnitOfWork unitOfWork,IMapper mapper ,ILogger<RoomTypeService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        
        //Lất tất cả loại
        public async Task<List<GetRoomTypeDTO>> GetAllRoomType()
        {
            // Lấy danh sách RoomType từ database
            List<RoomType> roomTypes = await _unitOfWork.GetRepository<RoomType>()
                .Entities.Where(r => r.DeletedTime == null).ToListAsync();
            foreach (var item in roomTypes)
            {
                // Khởi tạo ImageRoomTypes nếu nó chưa được khởi tạo
                item.ImageRoomTypes = new List<ImageRoomType>();

                // Lấy danh sách hình ảnh tương ứng với RoomType
                var listImage = _unitOfWork.GetRepository<ImageRoomType>()
                    .Entities.Where(i => i.RoomTypeID == item.Id).ToList();

                // Thêm từng ảnh vào RoomType.ImageRoomTypes
                if (listImage != null)
                {
                    foreach (var image in listImage)
                    {
                        if (!item.ImageRoomTypes.Any(i => i.ImageID == image.ImageID))
                        {
                            item.ImageRoomTypes.Add(image);
                        }
                    }
                }
            }
            List<GetRoomTypeDTO> list = new List<GetRoomTypeDTO>();
            foreach(var item in roomTypes)
            {

                var roomTypeDTO = new GetRoomTypeDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,

                };
                if(item.ImageRoomTypes!=null)
                {
                    roomTypeDTO.ImageRoomTypes = new List<GetImageRoomTypeDTO>();
                    foreach (var image in item.ImageRoomTypes)
                    {
                        var imageDTO = await _unitOfWork.GetRepository<Image>().Entities.FirstOrDefaultAsync(i => i.Id == image.ImageID);
                        if(imageDTO!=null)
                        {
                            var imageRoomType = new GetImageRoomTypeDTO()
                            {
                                URL = imageDTO.URL
                            };
                            if (!roomTypeDTO.ImageRoomTypes.Any(i => i.URL == imageRoomType.URL))
                            {
                                roomTypeDTO.ImageRoomTypes.Add(imageRoomType);
                            }
                        }
                    }
                }
                list.Add(roomTypeDTO);
            }
            return list;
        }
        //Tìm kiếm theo id
        public async Task<GetRoomTypeDTO> GetRoomTypeById(string id)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");
            if (!regex.IsMatch(id.Trim()))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID không hợp lệ! Không được chứa ký tự đặc biệt.");
            }
            var roomType = await _unitOfWork.GetRepository<RoomType>().Entities.FirstOrDefaultAsync(r=>r.Id==id);
            if(roomType == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng!");
            }    
            GetRoomTypeDTO roomTypeDTO = _mapper.Map<GetRoomTypeDTO>(roomType);
            var imageList=await _unitOfWork.GetRepository<ImageRoomType>().Entities.Where(i=>i.RoomTypeID==id).ToListAsync();
            if (imageList != null)
            {
                roomTypeDTO.ImageRoomTypes = new List<GetImageRoomTypeDTO>();
                foreach (var image in imageList)
                {
                    var imageDTO = await _unitOfWork.GetRepository<Image>().Entities.FirstOrDefaultAsync(i => i.Id == image.ImageID);
                    if (imageDTO != null)
                    {
                        var imageRoomType = new GetImageRoomTypeDTO()
                        {
                            URL = imageDTO.URL
                        };
                        if (!roomTypeDTO.ImageRoomTypes.Any(i => i.URL == imageRoomType.URL))
                        {
                            roomTypeDTO.ImageRoomTypes.Add(imageRoomType);
                        }
                    }
                }
            }
            return roomTypeDTO;
        }
        public async Task<RoomType> CreateRoomType(PortRoomTypeDTO model)
        {
            // Kiểm tra xem model có hợp lệ không
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest,
                    ResponseCodeConstants.INVALID_INPUT,
                    "Tên loại phòng là bắt buộc.");
            }
            try
            {
                string userID = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
                RoomType roomType = _mapper.Map<RoomType>(model);
                roomType.CreatedBy = userID;
                roomType.LastUpdatedBy = userID;

                await _unitOfWork.GetRepository<RoomType>().InsertAsync(roomType);
                await _unitOfWork.SaveChangesAsync();

                return roomType;
            }
            catch (Exception ex)
            {
                throw new ErrorException(StatusCodes.Status500InternalServerError,
                    ResponseCodeConstants.INTERNAL_SERVER_ERROR,
                    "Đã xảy ra lỗi trong quá trình tạo loại phòng: " + ex.Message);
            }
        }
        public async Task DeleteRoomType(string id)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");
            if (!regex.IsMatch(id.Trim()))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID không hợp lệ! Không được chứa ký tự đặc biệt.");
            }
            RoomType roomType=await _unitOfWork.GetRepository<RoomType>().Entities.FirstOrDefaultAsync(r=>r.Id ==id && r.DeletedTime==null)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng!");

            string userID = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);

            roomType.DeletedTime = CoreHelper.SystemTimeNow;
            roomType.DeletedBy = userID;
            await _unitOfWork.GetRepository<RoomType>().UpdateAsync(roomType);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
