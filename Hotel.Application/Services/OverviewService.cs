using AutoMapper;
using Hotel.Application.DTOs.OverviewDTO;
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
    public class OverviewService : IOverviewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private string currenUser => Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

        public OverviewService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }

        public async Task<PaginatedList<GetOverviewDTO>> GetPageAsync(int index, int pageSize, string idSearch, string customerID)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }
            IQueryable<Overview> query = _unitOfWork.GetRepository<Overview>().Entities
               .Where(c => !c.DeletedTime.HasValue)
               .OrderByDescending(c => c.CreatedTime);

            if(string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(o => o.Id.Equals(idSearch));
            }

            if(string.IsNullOrWhiteSpace(customerID))
            {
                query = query.Where(o => o.CustomerId.Equals(customerID));
            }

            int totalCount = await query.CountAsync();
            if (totalCount == 0)
            {
                return new PaginatedList<GetOverviewDTO>(new List<GetOverviewDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            //Mapping dữ liệu

            List<GetOverviewDTO> responseItems = _mapper.Map<List<GetOverviewDTO>>(resultQuery);

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            PaginatedList<GetOverviewDTO> responsePaginatedList = new PaginatedList<GetOverviewDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );
            return responsePaginatedList;
        }
        //Lấy điểm trung bình của đánh giá
        public async Task<GetOverviewDTO> GetAvgOverview()
        {
            // Lấy danh sách các Overview và tính toán trung bình cộng của từng điểm
            var overviews = await _unitOfWork.GetRepository<Overview>().Entities
                .Where(c => !c.DeletedTime.HasValue)
                .Select(c => new 
                {
                    c.EmployeePoint,
                    c.ComfortPoint,
                    c.ClearPoint,
                    c.ServicePoint,
                    c.ViewPoint,
                    c.RoomPoint
                })
                .ToListAsync();

            // Tính trung bình cộng cho từng loại điểm
            var avgEmployeePoint = overviews.Average(o => o.EmployeePoint);
            var avgComfortPoint = overviews.Average(o => o.ComfortPoint);
            var avgClearPoint = overviews.Average(o => o.ClearPoint);
            var avgServicePoint = overviews.Average(o => o.ServicePoint);
            var avgViewPoint = overviews.Average(o => o.ViewPoint);
            var avgRoomPoint = overviews.Average(o => o.RoomPoint);

            // Tạo DTO để trả về
            return new GetOverviewDTO
            {
                EmployeePoint = (float)Math.Round(avgEmployeePoint),
                ComfortPoint = (float)Math.Round(avgComfortPoint),
                ClearPoint = (float)Math.Round(avgClearPoint),
                ServicePoint = (float)Math.Round(avgServicePoint),
                ViewPoint = (float)Math.Round(avgViewPoint),
                RoomPoint = (float)Math.Round(avgRoomPoint),
                Id = "Average", 
            };
        }
        //Tạo đánh giá
        public async Task CreateOverview(PostOverviewDTO model)
        {
            //Kiểm tra Id khách hàng
            Customer customer = await _unitOfWork.GetRepository<Customer>().Entities.Where(c => !c.DeletedTime.HasValue).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Mày là thèn nào");

            Booking booking =await _unitOfWork.GetRepository<Booking>().Entities.Where(b=>b.CustomerId.Equals(model.CustomerId) && !b.DeletedTime.HasValue).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không thể đánh giá khi chưa sử dụng dịch vụ của khách sạn");
            //Gán thông tin
            Overview overview = _mapper.Map<Overview>(model);

            overview.CreatedBy = currenUser;
            overview.LastUpdatedBy= currenUser;

            await _unitOfWork.GetRepository<Overview>().InsertAsync(overview);
            await _unitOfWork.SaveChangesAsync();
        }
        //Update đánh giá
        public async Task UpdateOverview(string id, PutOverviewDTO model)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập Id");
            }

            //Tìm đánh gia theo Id
            Overview overview = await _unitOfWork.GetRepository<Overview>().Entities.Where(o=>o.Id.Equals(id)).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy đánh giá");

            //Cập nhật lại thông tin
            overview = _mapper.Map<Overview>(model);

            overview.LastUpdatedTime = CoreHelper.SystemTimeNow;
            overview.LastUpdatedBy = currenUser;

            await _unitOfWork.GetRepository<Overview>().InsertAsync(overview);
            await _unitOfWork.SaveChangesAsync();
        }
        //Xoá đánh giá
        public async Task DeleteOverview(string id)
        {
            //Validate input
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng nhập Id");
            }
            //Kiểm tra thử đánh giá có tồn tại không
            Overview overview = await _unitOfWork.GetRepository<Overview>().Entities.Where(o => o.Id.Equals(id)).FirstOrDefaultAsync()
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy đánh giá");

            overview.DeletedTime = CoreHelper.SystemTimeNow;
            overview.DeletedBy = currenUser;

            await _unitOfWork.GetRepository<Overview>().InsertAsync(overview);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
