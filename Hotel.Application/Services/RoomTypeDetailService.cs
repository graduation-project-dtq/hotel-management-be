
using AutoMapper;
using Hotel.Application.DTOs.ImageDTO;
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
        public async Task<GetRoomTypeDetailDTO> GetById(string id)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");
            if (!regex.IsMatch(id.Trim()))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID không hợp lệ! Không được chứa ký tự đặc biệt.");
            }

            RoomTypeDetail roomTypeDetail= await _unitOfWork.GetRepository<RoomTypeDetail>().
                Entities.FirstOrDefaultAsync(r=>r.Id== id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng");

            GetRoomTypeDetailDTO getRoomType = new GetRoomTypeDetailDTO()
            {
                Id = roomTypeDetail.Id,
                Name = roomTypeDetail.Name,
                Description = roomTypeDetail.Description,
                CapacityMax = roomTypeDetail.CapacityMax,
                BasePrice=roomTypeDetail.BasePrice,
                DiscountPrice=roomTypeDetail.BasePrice,
                Area = roomTypeDetail.Area,
                AverageStart = roomTypeDetail.AverageStart,
            };
            decimal discount = await GetDiscountPrice(roomTypeDetail.Id);
            if(discount > 0)
            {
                getRoomType.DiscountPrice=discount;
            }    
            getRoomType.ImageRoomTypeDetailDTOs = new List<GetImageRoomTypeDetailDTO>();

            roomTypeDetail.ImageRoomTypeDetails = new List<ImageRoomTypeDetail>();
            List<ImageRoomTypeDetail> listImage=await _unitOfWork.GetRepository<ImageRoomTypeDetail>().Entities.Where(i=>i.RoomTypeDetailID==roomTypeDetail.Id).ToListAsync();
            if (listImage.Count > 0)
            {
                //Gán vào 
                foreach (var image in listImage)
                {
                    GetImageRoomTypeDetailDTO detailImage = new GetImageRoomTypeDetailDTO();
                    //Tìm ảnh
                    Image imageURL = await _unitOfWork.GetRepository<Image>().GetByIdAsync(image.ImageID);
                    if (imageURL != null)
                    {
                        detailImage.URL = imageURL.URL;
                    }
                    getRoomType.ImageRoomTypeDetailDTOs.Add(detailImage);
                }

            }
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
            List<RoomTypeDetail> roomTypeDetails =
                await _unitOfWork.GetRepository<RoomTypeDetail>().Entities
                .Include(r=>r.RoomPriceAdjustments)
                .Where(r=>r.RoomTypeID==id).ToListAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại theo ID!");
            //Gắn thêm hình vào
            foreach (var item in roomTypeDetails)
            {
               
                item.ImageRoomTypeDetails = new List<ImageRoomTypeDetail>();
                var listImage = _unitOfWork.GetRepository<ImageRoomTypeDetail>()
                    .Entities.Where(i => i.RoomTypeDetailID == item.Id).ToList();

                // Thêm từng ảnh vào 
                if (listImage != null)
                {
                    foreach (var image in listImage)
                    {
                        if (!item.ImageRoomTypeDetails.Any(i => i.ImageID == image.ImageID))
                        {
                            item.ImageRoomTypeDetails.Add(image);
                        }
                    }
                }
            }

            List<GetRoomTypeDetailDTO> list = new List<GetRoomTypeDetailDTO>();

            foreach (var item in roomTypeDetails)
            {

                var roomTypeDetailDTO = new GetRoomTypeDetailDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CapacityMax= item.CapacityMax,
                    BasePrice= item.BasePrice,
                    DiscountPrice=item.BasePrice,
                    Area=item.Area,
                    AverageStart= item.AverageStart,
                };
                if (item.ImageRoomTypeDetails != null)
                {
                    roomTypeDetailDTO.ImageRoomTypeDetailDTOs = new List<GetImageRoomTypeDetailDTO>();
                    foreach (var image in item.ImageRoomTypeDetails)
                    {
                        var imageDTO = await _unitOfWork.GetRepository<Image>().Entities.FirstOrDefaultAsync(i => i.Id == image.ImageID);
                        if (imageDTO != null)
                        {
                            var imageRoomTypeDetail = new GetImageRoomTypeDetailDTO()
                            {
                                URL = imageDTO.URL
                            };
                            if (!roomTypeDetailDTO.ImageRoomTypeDetailDTOs.Any(i => i.URL == imageRoomTypeDetail.URL))
                            {
                                roomTypeDetailDTO.ImageRoomTypeDetailDTOs.Add(imageRoomTypeDetail);
                            }
                        }
                    }
                }   
                if (item.RoomPriceAdjustments != null)
                {
                    decimal discount = await GetDiscountPrice(roomTypeDetailDTO.Id);
                    if (discount != 0)
                    {
                        roomTypeDetailDTO.DiscountPrice=discount;
                    }
                }
                list.Add(roomTypeDetailDTO);
            }
            return list;
        }

        //Tạo loại phòng mới
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


        //TÌm phòng còn trống
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
                .Take(3) // Lấy tổ hợp có sức chứa tối thiểu
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
        public async Task<decimal> GetDiscountPrice(string roomTypeDetailId)
        {
            List<RoomPriceAdjustment> roomPriceAdjustments = await _unitOfWork.GetRepository<RoomPriceAdjustment>().Entities.Where(r => r.RoomTypeDetailId == roomTypeDetailId).ToListAsync();
            if(roomPriceAdjustments==null && roomPriceAdjustments.Count == 0)
            {
                return 0;
            }
            List<PriceAdjustmentPlan> adjustmentPlans = new List<PriceAdjustmentPlan>();
            foreach (var item in roomPriceAdjustments)
            {
                PriceAdjustmentPlan adjustmentPlan =await _unitOfWork.GetRepository<PriceAdjustmentPlan>().GetByIdAsync(item.PriceAdjustmentPlanId);
                if(adjustmentPlan!=null)
                {
                    adjustmentPlans.Add(adjustmentPlan);
                }    
            }
            if(adjustmentPlans==null)
            {
                return 0;
            }

            PriceAdjustmentPlan discountPlan = adjustmentPlans
                        .Where(p => p.StartDate <= CoreHelper.SystemTimeNow && p.EndDate >= CoreHelper.SystemTimeNow)
                        .OrderByDescending(p => p.StartDate)
                        .FirstOrDefault();
            if (discountPlan == null)
            {
                return 0;
            }
            return discountPlan.AdjustmentValue;
        }

       
    }
}
