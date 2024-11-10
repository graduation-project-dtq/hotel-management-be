using AutoMapper;
using Hotel.Application.DTOs.CustomerDTO;
using Hotel.Application.DTOs.EvaluationDTO;
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
    public class EvaluationService : IEvaluationService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IFirebaseService _firebaseService;
        private string curenUserId => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
        public EvaluationService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IFirebaseService firebaseService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _firebaseService = firebaseService;
        }
        public async Task<PaginatedList<GetEvaluationDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID, string roomTypeId)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }
            IQueryable<Evaluation> query = _unitOfWork.GetRepository<Evaluation>().Entities
               .Where(e=>!e.DeletedTime.HasValue)
               .OrderByDescending(c => c.CreatedTime);
            //Tìm theo Id
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
            }
            //Tìm theo khách hàng
            if (!string.IsNullOrWhiteSpace(customerID))
            {
                query = query.Where(r => r.CustomerId.ToString() == customerID);
            }
            //Tìm theo loại
            if (!string.IsNullOrWhiteSpace(roomTypeId))
            {
                query = query.Where(r => r.RoomTypeId.ToString() == roomTypeId);
            }

            var totalCount =await  query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetEvaluationDTO>(new List<GetEvaluationDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            List<GetEvaluationDTO> responseItems = query
               .Select(e => new GetEvaluationDTO
               {
                   //Map thuộc tính
                   Id = e.Id,
                   CustomerId = e.CustomerId,
                   Comment = e.Comment,
                   RoomTypeId = e.RoomTypeId,
                   Starts= e.Starts,
                   Images = e.ImageEvaluations != null ? e.ImageEvaluations.Select(img => new GetImage()
                   {
                       URL = img.Image != null && img.Image.URL != null
                           ? img.Image.URL
                           : string.Empty
                   }).ToList() : new List<GetImage>()
               })
               .ToList();

            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetEvaluationDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;

        }
        public async Task<List<GetEvaluationDTO>> GetEvaluationAsync(string roomTypeId)
        {
            if (string.IsNullOrWhiteSpace(roomTypeId))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng chọn loại phòng");
            }

            //Lấy dữ liệu
            List<GetEvaluationDTO> evaluations = await _unitOfWork.GetRepository<Evaluation>()
                .Entities
                .Where(e => !e.DeletedTime.HasValue && e.RoomTypeId == roomTypeId)
                .OrderByDescending(e => e.LastUpdatedTime)
                .Select(e => new GetEvaluationDTO
                {
                    //Map thuộc tính
                    Id = e.Id,
                    CustomerId = e.CustomerId,
                    Comment = e.Comment,
                    RoomTypeId = e.RoomTypeId,
                    Starts = e.Starts,
                    Customer = _mapper.Map<GetCustomerDTO>(e.Customer != null ? e.Customer : null),
                    Images = e.ImageEvaluations != null ? e.ImageEvaluations.Select(img => new GetImage()
                    {
                        URL = img.Image != null && img.Image.URL != null
                            ? img.Image.URL
                            : string.Empty
                    }).ToList() : new List<GetImage>()
                })
                .ToListAsync();

            return evaluations;
        }
        
        //Tạo đánh giá
        public async Task CreateEvaluationAsync(ICollection<IFormFile>? images, PostEvaluationDTO model)
        {
            Customer customer = await _unitOfWork.GetRepository<Customer>().Entities.Where(c => c.Id.Equals(model.CustomerId)
                && !c.DeletedTime.HasValue).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Khách hàng không tồn tại");

            RoomType roomType = await _unitOfWork.GetRepository<RoomType>().Entities.Where(r => r.Id.Equals(model.RoomTypeId)
               && !r.DeletedTime.HasValue).FirstOrDefaultAsync()
               ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Loại phòng không tồn tại");

            //Kiểm tra có thuê phòng hay chưa
            var check = await _unitOfWork.GetRepository<Booking>()
                .Entities
                .Where(b => b.CustomerId.Equals(model.CustomerId) && !b.DeletedTime.HasValue)
                .Include(b => b.BookingDetails!)
                    .ThenInclude(bd => bd.Room!)
                        .ThenInclude(r => r.RoomTypeDetail!)
                            .ThenInclude(rt=>rt.RoomType!)
                .FirstOrDefaultAsync(b => b.BookingDetails != null && b.BookingDetails
                    .Any(bd => bd.Room != null && bd.Room.RoomTypeDetail.RoomType != null && bd.Room.RoomTypeDetail.RoomType.Id == model.RoomTypeId))
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Bạn chưa thuê phòng nên không thể đánh giá!");

            //Thêm đánh giá
            Evaluation evaluation = _mapper.Map<Evaluation>(model);
            evaluation.CreatedBy = evaluation.LastUpdatedBy = curenUserId;

            roomType.AverageStart = (roomType.AverageStart + model.Starts) / 2;

            await _unitOfWork.GetRepository<RoomType>().InsertAsync(roomType);
            await _unitOfWork.GetRepository<Evaluation>().InsertAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();
            //Thêm hình ảnh
            if (images != null)
            {
                foreach (var item in images)
                {
                    //Upload hình ảnh
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
                    ImageEvaluation imageEvaluation = new ImageEvaluation()
                    {
                        ImageID = image.Id,
                        EvaluationID = evaluation.Id,
                    };

                    await _unitOfWork.GetRepository<ImageEvaluation>().InsertAsync(imageEvaluation);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

        }
        //Update đánh giá
        public async Task UpdateEvaluationAsync(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập mã đánh giá");
            }

            Evaluation evaluation = await _unitOfWork.GetRepository<Evaluation>().Entities.Where(e=>e.Id.Equals(id) && !e.DeletedTime.HasValue).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy đánh giá");

            evaluation.DeletedTime = CoreHelper.SystemTimeNow;
            evaluation.DeletedBy = curenUserId;

            await _unitOfWork.GetRepository<Evaluation>().InsertAsync(evaluation);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task PutEvaluationAsync(PostEvaluationDTO model)
        {

        }

    }
}
