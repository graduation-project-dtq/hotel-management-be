
using AutoMapper;
using Hotel.Application.DTOs.RoomTypeDetailDTO;
using Hotel.Application.DTOs.RoomTypeDTO;
using Hotel.Application.Extensions;
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
    public class RoomTypeDetailService : IRoomTypeDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<RoomTypeDetailService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoomTypeDetailService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomTypeDetailService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<GetRoomTypeDetailDTO>> GetAllRoomTypeDetail()
        {
            List<GetRoomTypeDetailDTO> roomTypeDetails = _mapper.Map<List<GetRoomTypeDetailDTO>>(await _unitOfWork.GetRepository<RoomTypeDetail>()
              .Entities.Where(r => r.DeletedTime == null).ToListAsync());
            return roomTypeDetails;
        }

        //Tìm kiếm theo ID
        public async Task<GetRoomTypeDetailDTO>GetRoomTypeDetailById(string id)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");
            if (!regex.IsMatch(id.Trim()))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID không hợp lệ! Không được chứa ký tự đặc biệt.");
            }

            var roomTypeDetail= await _unitOfWork.GetRepository<RoomTypeDetail>().
                Entities.FirstOrDefaultAsync(r=>r.Id== id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng");

            GetRoomTypeDetailDTO getRoomType = _mapper.Map<GetRoomTypeDetailDTO>(roomTypeDetail);

            return getRoomType;
        }

        //Tìm kiếm theo RoomTypeID

        public async Task<List<GetRoomTypeDetailDTO>> GetByRoomTypeId(string id)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");
            if (!regex.IsMatch(id.Trim()))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID không hợp lệ! Không được chứa ký tự đặc biệt.");
            }
            List<GetRoomTypeDetailDTO> roomTypeDetails = _mapper.Map<List<GetRoomTypeDetailDTO>>
                (await _unitOfWork.GetRepository<RoomTypeDetail>().Entities.Where(r=>r.RoomTypeID==id).ToListAsync())
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại theo ID!");

            return roomTypeDetails;
        }
        public async Task<RoomTypeDetail> CreateRoomTypeDetail(PostRoomTypeDetailDTO portRoomTypeDetail)
        {
            var roomtype = await _unitOfWork.GetRepository<RoomType>()
                .Entities.FirstOrDefaultAsync(r => r.Id == portRoomTypeDetail.RoomTypeID);

            if (roomtype == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.EXISTED, "Không tồn tại loại phòng!");
            }

            var roomTypeDetailExit = await _unitOfWork.GetRepository<RoomTypeDetail>()
                .Entities.FirstOrDefaultAsync(r => r.Name == portRoomTypeDetail.Name);

            if (roomTypeDetailExit != null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.DUPLICATE, "Chi tiết loại phòng đã tồn tại!");
            }

            try
            {
                string userID = Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
                RoomTypeDetail roomTypeDetail = _mapper.Map<RoomTypeDetail>(portRoomTypeDetail);

                roomTypeDetail.CreatedBy = userID;
                roomTypeDetail.LastUpdatedBy = userID;

                await _unitOfWork.GetRepository<RoomTypeDetail>().InsertAsync(roomTypeDetail);
                await _unitOfWork.SaveChangesAsync();

                return roomTypeDetail;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Lỗi khi lưu RoomTypeDetail: {Message}", dbEx.InnerException?.Message);
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, $"Thêm thất bại: {dbEx.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định: {Message}", ex.Message);
                throw new ErrorException(StatusCodes.Status500InternalServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Thêm thất bại!");
            }
        }



        public async Task<List<List<GetRoomTypeDetailDTO>>> FindRoom(int soNguoi,string roomTypeID)
        {
            if(String.IsNullOrWhiteSpace(roomTypeID))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Không được để trống số người ở!");
            }  
            if(String.IsNullOrWhiteSpace(roomTypeID))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Không được để trống số loại phòng!");

            }
            // Lấy danh sách phòng từ cơ sở dữ liệu mà không có DeletedTime
           var roomTypeDetails = await _unitOfWork.GetRepository<RoomTypeDetail>()
                .Entities
                .Where(r => r.DeletedTime == null && r.RoomTypeID == roomTypeID)
                .OrderByDescending(r=>r.CapacityMax)
                .ToListAsync();

            List<List<RoomTypeDetail>> output = new List<List<RoomTypeDetail>>();

        

            // Tìm các phòng sao cho tổng sức chứa >= số người cần
            TimKiemPhong(roomTypeDetails, new List<RoomTypeDetail>(), soNguoi, output);

            // Chọn tổ hợp phòng có tổng sức chứa gần nhất với số người yêu cầu và phù hợp nhất
            var ketQuaPhuHopNhat = output
                .Where(ketHop => ketHop.Sum(p => p.CapacityMax) >= soNguoi) // Chỉ giữ lại tổ hợp có sức chứa đủ
                .OrderBy(ketHop => ketHop.Sum(p => p.CapacityMax)) // Sắp xếp theo tổng sức chứa tăng dần
                .Take(2) // Lấy tổ hợp có sức chứa tối thiểu
                .ToList();

            var ketqua = _mapper.Map<List<List<GetRoomTypeDetailDTO>>>(ketQuaPhuHopNhat);
            return ketqua;
        }

        private  void TimKiemPhong(List<RoomTypeDetail> danhSachPhong, List<RoomTypeDetail> ketHopHienTai, int soNguoiCanTim, List<List<RoomTypeDetail>> ketQua)
        {
            // Nếu tổng số người trong kết hợp hiện tại >= số người cần, thêm vào kết quả
            if (ketHopHienTai.Sum(p => p.CapacityMax) >= soNguoiCanTim)
            {
                ketQua.Add(new List<RoomTypeDetail>(ketHopHienTai));
                return;
            }

            // Duyệt qua các phòng và tìm kết hợp
            for (int i = 0; i < danhSachPhong.Count; i++)
            {
                var phongHienTai = danhSachPhong[i];
                ketHopHienTai.Add(phongHienTai);
                TimKiemPhong(danhSachPhong.Skip(i + 1).ToList(), ketHopHienTai, soNguoiCanTim, ketQua);
                ketHopHienTai.RemoveAt(ketHopHienTai.Count - 1);
            }
        }

    }
}
