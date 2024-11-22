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
        private readonly IFirebaseService _firebaseService;
        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_httpContextAccessor);
        public RoomTypeDetailService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoomTypeDetailService> logger
            , IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _firebaseService = firebaseService;
        }
        public async Task<List<GetRoomTypeDetailDTO>> GetAllRoomTypeDetail()
        {
            List<RoomTypeDetail> roomTypeDetails =
                await _unitOfWork.GetRepository<RoomTypeDetail>().Entities
                .Include(r => r.RoomType)
                .Where(r => !r.DeletedTime.HasValue).ToListAsync();
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
                    CapacityMax = item.CapacityMax,
                    BasePrice = item.BasePrice,
                    DiscountPrice = item.BasePrice,
                    RoomTypeID = item.RoomType != null ? item.RoomType.Id : null,
                    Area = item.Area,
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
                decimal discount = await GetDiscountPrice(roomTypeDetailDTO.Id);
                if (discount > 0)
                {
                    roomTypeDetailDTO.DiscountPrice = roomTypeDetailDTO.BasePrice- discount;
                }
                list.Add(roomTypeDetailDTO);
            }
            return list;
        }

        //Tìm kiếm theo ID
        public async Task<GetRoomTypeDetailDTO> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng chọn loại phòng");
            }
            RoomTypeDetail roomTypeDetail = await _unitOfWork.GetRepository<RoomTypeDetail>()
                .Entities
                .Include(r=>r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng");

            GetRoomTypeDetailDTO getRoomType = new GetRoomTypeDetailDTO()
            {
                Id = roomTypeDetail.Id,
                Name = roomTypeDetail.Name,
                Description = roomTypeDetail.Description,
                CapacityMax = roomTypeDetail.CapacityMax,
                BasePrice = roomTypeDetail.BasePrice,
                DiscountPrice = roomTypeDetail.BasePrice,
                RoomTypeID = roomTypeDetail.RoomType != null ? roomTypeDetail.RoomType.Id : null,
                Area = roomTypeDetail.Area,
            };
            decimal discount = await GetDiscountPrice(roomTypeDetail.Id);
            if (discount > 0)
            {
                getRoomType.DiscountPrice =getRoomType.BasePrice - discount;
            }
            getRoomType.ImageRoomTypeDetailDTOs = new List<GetImageRoomTypeDetailDTO>();

            roomTypeDetail.ImageRoomTypeDetails = new List<ImageRoomTypeDetail>();
            List<ImageRoomTypeDetail> listImage = await _unitOfWork.GetRepository<ImageRoomTypeDetail>().Entities.Where(i => i.RoomTypeDetailID == roomTypeDetail.Id).ToListAsync();
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
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng chọn loại phòng");
            }
            List<RoomTypeDetail> roomTypeDetails =
                await _unitOfWork.GetRepository<RoomTypeDetail>().Entities
                .Include(r => r.RoomType)
                .Where(r => r.RoomTypeID == id).ToListAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng!");
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

                GetRoomTypeDetailDTO roomTypeDetailDTO = new GetRoomTypeDetailDTO()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    CapacityMax = item.CapacityMax,
                    BasePrice = item.BasePrice,
                    DiscountPrice = item.BasePrice,
                    RoomTypeID= item.RoomType!=null ? item.RoomType.Id : null,
                    Area = item.Area,
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
                decimal discount = await GetDiscountPrice(item.Id);
                if (discount > 0)
                {
                    roomTypeDetailDTO.DiscountPrice =roomTypeDetailDTO.BasePrice - discount;
                }
                list.Add(roomTypeDetailDTO);
            }
            return list;
        }

        //Tạo loại phòng mới
        public async Task<GetRoomTypeDetailDTO> CreateRoomTypeDetail(ICollection<IFormFile>? images, PostRoomTypeDetailDTO model)
        {
            RoomType roomType = await _unitOfWork.GetRepository<RoomType>().Entities
                .Where(r => r.Id.Equals(model.RoomTypeID) && !r.DeletedTime.HasValue)
                .FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.EXISTED, "Không tồn tại loại phòng!");

            RoomTypeDetail? roomTypeDetailExit = await _unitOfWork.GetRepository<RoomTypeDetail>()
                .Entities.FirstOrDefaultAsync(r => r.Name == model.Name);

            if (roomTypeDetailExit != null)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.DUPLICATE, "Chi tiết loại phòng đã tồn tại!");
            }
            RoomTypeDetail roomTypeDetail = _mapper.Map<RoomTypeDetail>(model);

            roomTypeDetail.CreatedBy = currentUserId;
            roomTypeDetail.LastUpdatedBy = currentUserId;

            await _unitOfWork.GetRepository<RoomTypeDetail>().InsertAsync(roomTypeDetail);
            await _unitOfWork.SaveChangesAsync();


            //Thêm hình ảnh
            if (images != null)
            {
                foreach (var item in images)
                {
                    PostImageViewModel postImageView = new PostImageViewModel()
                    {
                        File = item
                    };
                    string url = await _firebaseService.UploadFileAsync(postImageView);

                    Image image = new Image()
                    {
                        URL = url,
                        CreatedBy = currentUserId,
                        LastUpdatedBy = currentUserId,
                    };

                    await _unitOfWork.GetRepository<Image>().InsertAsync(image);
                    await _unitOfWork.SaveChangesAsync();

                    ImageRoomTypeDetail imageRoomType = new ImageRoomTypeDetail()
                    {
                        ImageID = image.Id,
                        RoomTypeDetailID = roomTypeDetail.Id
                    };
                    await _unitOfWork.GetRepository<ImageRoomTypeDetail>().InsertAsync(imageRoomType);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            RoomTypeDetail? resultRoomType = await _unitOfWork.GetRepository<RoomTypeDetail>().Entities
                .Where(r => r.Id.Equals(roomTypeDetail.Id) && !r.DeletedTime.HasValue).FirstOrDefaultAsync();
            GetRoomTypeDetailDTO getRoomTypeDetailDTO = _mapper.Map<GetRoomTypeDetailDTO>(resultRoomType);

            getRoomTypeDetailDTO.DiscountPrice = resultRoomType!=null ? resultRoomType.BasePrice : 0;
            getRoomTypeDetailDTO.ImageRoomTypeDetailDTOs = resultRoomType.ImageRoomTypeDetails != null ?
                               resultRoomType.ImageRoomTypeDetails.Select(img => new GetImageRoomTypeDetailDTO()
                               {
                                   URL = img.Image != null ? img.Image.URL : string.Empty,
                               }).ToList()
                               : new List<GetImageRoomTypeDetailDTO>();


            return getRoomTypeDetailDTO;
        }


        //TÌm phòng còn trống
        public async Task<List<List<GetRoomTypeDetailDTO>>> FindRoom(int soNguoi, string roomTypeID)
        {

            if (String.IsNullOrWhiteSpace(roomTypeID))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng chọn loại phòng!");

            }
            // Lấy danh sách phòng từ cơ sở dữ liệu mà không có DeletedTime
            var roomTypeDetails = await _unitOfWork.GetRepository<RoomTypeDetail>()
                 .Entities
                 .Where(r => r.DeletedTime == null && r.RoomTypeID == roomTypeID)
                 .OrderByDescending(r => r.CapacityMax)
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

        private void TimKiemPhong(List<RoomTypeDetail> danhSachPhong, List<RoomTypeDetail> ketHopHienTai, int soNguoiCanTim, List<List<RoomTypeDetail>> ketQua)
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
            if (roomPriceAdjustments == null )
            {
                return 0;
            }
            List<PriceAdjustmentPlan> adjustmentPlans = new List<PriceAdjustmentPlan>();
            foreach (var item in roomPriceAdjustments)
            {
                PriceAdjustmentPlan adjustmentPlan = await _unitOfWork.GetRepository<PriceAdjustmentPlan>().GetByIdAsync(item.PriceAdjustmentPlanId);
                if (adjustmentPlan != null)
                {
                    adjustmentPlans.Add(adjustmentPlan);
                }
            }
            if (adjustmentPlans == null)
            {
                return 0;
            }

            PriceAdjustmentPlan? discountPlan = adjustmentPlans
                        .Where(p => p.StartDate <= CoreHelper.SystemTimeNow && p.EndDate >= CoreHelper.SystemTimeNow)
                        .OrderByDescending(p => p.StartDate)
                        .FirstOrDefault();
            if (discountPlan == null)
            {
                return 0;
            }
            return discountPlan.AdjustmentValue;
        }
        public async Task UpdateRoomTypeDetail(string id, ICollection<IFormFile>? images, PutRoomTypeDetailDTO model)
        {
            var strategy = _unitOfWork.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.BeginTransactionAsync();

                if (string.IsNullOrWhiteSpace(id))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng chọn loại phòng!");
                }

                RoomTypeDetail roomTypeDetail = await _unitOfWork.GetRepository<RoomTypeDetail>().Entities
                    .FirstOrDefaultAsync(r => r.Id.Equals(id) && !r.DeletedTime.HasValue)
                    ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng!");

                if (!string.IsNullOrWhiteSpace(model.RoomTypeID))
                {
                    RoomType roomType = await _unitOfWork.GetRepository<RoomType>().Entities
                        .FirstOrDefaultAsync(r => r.Id.Equals(model.RoomTypeID) && !r.DeletedTime.HasValue)
                        ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy loại phòng (Lớn)!");
                    roomTypeDetail.RoomTypeID = model.RoomTypeID;
                }

                if (images != null)
                {
                    List<ImageRoomTypeDetail> imageRoomTypeDetail = await _unitOfWork.GetRepository<ImageRoomTypeDetail>()
                        .Entities
                        .Where(i => i.RoomTypeDetailID.Equals(id))
                        .ToListAsync();

                    if (imageRoomTypeDetail.Any())
                    {
                        await _unitOfWork.GetRepository<ImageRoomTypeDetail>().DeleteRangeAsync(imageRoomTypeDetail);
                        await _unitOfWork.SaveChangesAsync();
                    }

                    foreach (var item in images)
                    {
                        PostImageViewModel postImageView = new PostImageViewModel()
                        {
                            File = item
                        };
                        string url = await _firebaseService.UploadFileAsync(postImageView);

                        Image image = new Image()
                        {
                            URL = url,
                            CreatedBy = currentUserId,
                            LastUpdatedBy = currentUserId,
                        };

                        await _unitOfWork.GetRepository<Image>().InsertAsync(image);
                        await _unitOfWork.SaveChangesAsync();

                        ImageRoomTypeDetail imageRoomType = new ImageRoomTypeDetail()
                        {
                            ImageID = image.Id,
                            RoomTypeDetailID = roomTypeDetail.Id
                        };
                        await _unitOfWork.GetRepository<ImageRoomTypeDetail>().InsertAsync(imageRoomType);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }

                roomTypeDetail.Name = model.Name ?? roomTypeDetail.Name;
                roomTypeDetail.CapacityMax = model.CapacityMax ?? roomTypeDetail.CapacityMax;
                roomTypeDetail.Area = model.Area ?? roomTypeDetail.Area;
                roomTypeDetail.Description = model.Description ?? roomTypeDetail.Description;
                roomTypeDetail.BasePrice = model.BasePrice ?? roomTypeDetail.BasePrice;
                roomTypeDetail.LastUpdatedBy = currentUserId;
                roomTypeDetail.LastUpdatedTime = CoreHelper.SystemTimeNow;

                await _unitOfWork.GetRepository<RoomTypeDetail>().UpdateAsync(roomTypeDetail);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();
            });
        }

        public async Task DeleteRoomTypeDetailAsync(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng chọn loại phòng");
            }
            RoomTypeDetail roomTypeDetail= await _unitOfWork.GetRepository<RoomTypeDetail>()
                .Entities.FirstOrDefaultAsync(r=>r.Id.Equals(id))
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy");
            roomTypeDetail.DeletedBy = currentUserId;
            roomTypeDetail.DeletedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<RoomTypeDetail>().UpdateAsync(roomTypeDetail);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
