
using AutoMapper;
using Hotel.Application.DTOs.EvaluationDTO;
using Hotel.Application.DTOs.FacilitiesDTO;
using Hotel.Application.DTOs.ImageDTO;
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

namespace Hotel.Application.Services
{
    public class FacilitiesService  : IFacilitiesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFirebaseService _firebaseService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;


        private string currentUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        public FacilitiesService(IUnitOfWork unitOfWork,IFirebaseService firebaseService, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _firebaseService = firebaseService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }
        public async Task<PaginatedList<GetFacilitiesDTO>> GetPageAsync(int index, int pageSize, string idSearch,
          string nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }
            IQueryable<Facilities> query = _unitOfWork.GetRepository<Facilities>().Entities
               .Where(e => !e.DeletedTime.HasValue)
               .OrderByDescending(c => c.CreatedTime);

            //Tìm theo Id
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.Equals(idSearch) );
            }
            //Tìm theo tên
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                query = query.Where(r => r.Name.Contains(nameSearch));
            }

            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetFacilitiesDTO>(new List<GetFacilitiesDTO>(), totalCount, index, pageSize);
            }
            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            List<GetFacilitiesDTO> responseItems = query
             .Select(e => new GetFacilitiesDTO
             {
                 //Map thuộc tính
                 Id = e.Id,
                 Name=e.Name,
                 Description=e.Description,
                 Price=e.Price,
                 Images = e.ImageFacilities != null ? e.ImageFacilities.Select(img => new GetImage()
                 {
                     URL = img.Image != null && img.Image.URL != null
                         ? img.Image.URL
                         : string.Empty
                 }).ToList() : new List<GetImage>()
             })
             .ToList();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetFacilitiesDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }
        //Lấy nội thất theo phòng
        public async Task<PaginatedList<GetFacilitiesRoomDTO>> GetFacilitiesByRoomId(int index, int pageSize, string roomId,
         string nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }
            if(string.IsNullOrWhiteSpace(roomId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng chọn phòng!");

            }
            IQueryable<FacilitiesRoom> query = _unitOfWork.GetRepository<FacilitiesRoom>().Entities
               .Where(e => e.Room.Id.Equals(roomId));

          
            //Tìm theo tên
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                query = query.Where(r => r.Facilities.Name.Contains( nameSearch));
            }

            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetFacilitiesRoomDTO>(new List<GetFacilitiesRoomDTO>(), totalCount, index, pageSize);
            }
            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            List<GetFacilitiesRoomDTO> responseItems = query
             .Select(e => new GetFacilitiesRoomDTO
             {
                 //Map thuộc tính
                 roomId=e.RoomID,
                 roomName=e.Room.Name,
                 Name = e.Facilities.Name,
                 Description = e.Facilities.Description,
                 Price = e.Facilities.Price,
                 Images = e.Facilities.ImageFacilities != null ? e.Facilities.ImageFacilities.Select(img => new GetImage()
                 {
                     URL = img.Image != null && img.Image.URL != null
                         ? img.Image.URL
                         : string.Empty
                 }).ToList() : new List<GetImage>()
             })
             .ToList();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetFacilitiesRoomDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }
        public async Task<GetFacilitiesDTO> CreateFacilities(ICollection<IFormFile> ? images,PostFacilitiesDTO model)
        {
            Facilities ? exitFacilities = await _unitOfWork.GetRepository<Facilities>().Entities.Where(f => f.Name.Equals(model) && !f.DeletedTime.HasValue).FirstOrDefaultAsync();
            if(exitFacilities != null)
            {
                throw new ErrorException(StatusCodes.Status409Conflict, ResponseCodeConstants.DUPLICATE, "Đã tồn tại tên này");
            }

            if(model.Price<0 && model.Price %1 != 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Giá tiền không hợp lệ");

            }

            Facilities facilities = _mapper.Map<Facilities>(model);
            facilities.CreatedBy = currentUserId;
            facilities.LastUpdatedBy = currentUserId;

            await _unitOfWork.GetRepository<Facilities>().InsertAsync(facilities);
            await _unitOfWork.SaveChangesAsync();

            //Thêm hình ảnh
            if(images!= null)
            {
                foreach (var item in images)
                {
                    PostImageViewModel postImage = new PostImageViewModel()
                    {
                        File= item
                    };
                    string url = await _firebaseService.UploadFileAsync(postImage);
                    Image image = new Image()
                    {
                        URL=url,
                    };
                    await _unitOfWork.GetRepository<Image>().InsertAsync(image);
                    await _unitOfWork.SaveChangesAsync();

                    ImageFacilities imageFacilities = new ImageFacilities()
                    {
                        ImageID=image.Id,
                        FacilitiesID=facilities.Id,
                    };
                    await _unitOfWork.GetRepository<ImageFacilities>().InsertAsync(imageFacilities);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            //Trả dữ liệu ra
            GetFacilitiesDTO getFacilitiesDTO = _mapper.Map<GetFacilitiesDTO>(facilities);

            getFacilitiesDTO.Images = facilities.ImageFacilities != null ? facilities.ImageFacilities.Select(img => new GetImage()
            {
                URL = img.Image != null ? img.Image.URL : string.Empty
            }).ToList() : new List<GetImage>();
          
            return getFacilitiesDTO;
        }
        public async Task<GetFacilitiesDTO> UpdateFacilities(string id, PutFacilitiesDTO model, ICollection<IFormFile> ? images)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT
                    , "Vui lòng chọn trang thiết bị");
            }
            Facilities ? facilities = await _unitOfWork.GetRepository<Facilities>().Entities
                .Where(f => f.Id.Equals(id) && !f.DeletedTime.HasValue).FirstOrDefaultAsync();
            if (facilities == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND
                    , "Không tìm thấy trang thiết bị");
            }
            facilities = _mapper.Map<Facilities>(model);

            if(images!= null)
            {
                //Xoá ảnh củ
                List<ImageFacilities> imageFacilities = await _unitOfWork.GetRepository<ImageFacilities>()
                    .Entities.Where(i => i.FacilitiesID.Equals(id)).ToListAsync();
                if(imageFacilities!=null)
                {
                    foreach(ImageFacilities item in imageFacilities)
                    {
                        await _unitOfWork.GetRepository<ImageFacilities>().DeleteAsync(item.FacilitiesID);
                        await _unitOfWork.SaveChangesAsync();
                    }
                }
                foreach (var item in images)
                {
                    PostImageViewModel postImage = new PostImageViewModel()
                    {
                        File = item
                    };
                    string url = await _firebaseService.UploadFileAsync(postImage);
                    Image image = new Image()
                    {
                        URL = url,
                    };
                    await _unitOfWork.GetRepository<Image>().InsertAsync(image);
                    await _unitOfWork.SaveChangesAsync();

                    ImageFacilities imagef = new ImageFacilities()
                    {
                        ImageID = image.Id,
                        FacilitiesID = facilities.Id,
                    };
                    await _unitOfWork.GetRepository<ImageFacilities>().InsertAsync(imagef);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            await _unitOfWork.GetRepository<Facilities>().InsertAsync(facilities);
            await _unitOfWork.SaveChangesAsync();

            GetFacilitiesDTO getFacilitiesDTO = _mapper.Map<GetFacilitiesDTO>(facilities);

            getFacilitiesDTO.Images = facilities.ImageFacilities != null ? facilities.ImageFacilities.Select(img => new GetImage()
            {
                URL = img.Image != null ? img.Image.URL : string.Empty
            }).ToList() : new List<GetImage>();

            return getFacilitiesDTO;

        }
        public async Task DeleteFacilities(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng chọn nội thất!");
            }
            Facilities facilities = await _unitOfWork.GetRepository<Facilities>().Entities.FirstOrDefaultAsync(f => f.Id.Equals(id) && !f.DeletedTime.HasValue)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Nội thất không tồn tại!");

            facilities.DeletedBy = currentUserId;
            facilities.DeletedTime = CoreHelper.SystemTimeNow;

            List<FacilitiesRoom> facilitiesRooms = await _unitOfWork.GetRepository<FacilitiesRoom>().Entities
                .Where(f => f.FacilitiesID.Equals(id))
                .ToListAsync();

            await _unitOfWork.GetRepository<FacilitiesRoom>().DeleteRangeAsync(facilitiesRooms);
            await _unitOfWork.GetRepository<Facilities>().UpdateAsync(facilities);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
