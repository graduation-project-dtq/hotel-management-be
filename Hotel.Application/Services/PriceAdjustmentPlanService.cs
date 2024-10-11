using AutoMapper;
using Hotel.Application.DTOs.PriceAdjustmentPlanDTO;
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
    public class PriceAdjustmentPlanService : IPriceAdjustmentPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public PriceAdjustmentPlanService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }
        public async Task<PaginatedList<GetPriceAdjustmentPlanDTO>> GetPageAsync(int index, int pageSize, string idSearch, string nameSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<PriceAdjustmentPlan> query = _unitOfWork.GetRepository<PriceAdjustmentPlan>().Entities
                 .Where(c => !c.DeletedTime.HasValue)
                 .OrderByDescending(c => c.CreatedTime);

            // Áp dụng điều kiện tìm kiếm theo ID
            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                query = query.Where(r => r.Id.ToString() == idSearch);
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy kế hoạch điều chỉnh giá phòng với ID đã cho!");
                }
            }

            // Áp dụng điều kiện tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(nameSearch))
            {
                query = query.Where(r => r.Name.Contains(nameSearch));
                bool exists = await query.AnyAsync();
                if (!exists)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy kế hoạch điều chỉnh giá phòng tên đã cho!");
                }
            }

            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetPriceAdjustmentPlanDTO>(new List<GetPriceAdjustmentPlanDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            List<GetPriceAdjustmentPlanDTO> responseItems =_mapper.Map<List< GetPriceAdjustmentPlanDTO >>(resultQuery);
            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetPriceAdjustmentPlanDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );

            return responsePaginatedList;
        }
        public async Task CreateRoomPriceAdjustmentPlan(PostPriceAdjustmentPlanDTO model)
        {
            var priceAdjustmentPlan = await _unitOfWork.GetRepository<PriceAdjustmentPlan>().Entities.FirstOrDefaultAsync(p => p.Name.Equals(model.Name));
            if(priceAdjustmentPlan != null)
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.DUPLICATE, "Trùng tên kết hoạch điều chỉnh giá phòng!");
            }
            string userId= Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            PriceAdjustmentPlan plan=_mapper.Map<PriceAdjustmentPlan>(model);
            plan.CreatedBy = userId;
            plan.LastUpdatedBy= userId;
            plan.RoomPriceAdjustments = new List<RoomPriceAdjustment>();

            if (model.RoomPriceAdjustmentPlans != null && model.RoomPriceAdjustmentPlans.Count > 0)
            {
                foreach(var item in model.RoomPriceAdjustmentPlans)
                {
                    RoomPriceAdjustment roomPrice = new RoomPriceAdjustment()
                    {
                        PriceAdjustmentPlanId = plan.Id,
                        RoomTypeDetailId=item.RoomTypeDetailId,
                    };
                    plan.RoomPriceAdjustments.Add(roomPrice);
                    await _unitOfWork.GetRepository<RoomPriceAdjustment>().InsertAsync(roomPrice);
                }
            }
            await _unitOfWork.GetRepository<PriceAdjustmentPlan>().InsertAsync(plan);

            await _unitOfWork.SaveChangesAsync();
           
            
        }
        public async Task UpdateRoomPriceAdjustmentPlan(string id, PutPriceAdjustmentPlanDTO model)
        {
            if(String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Vui lòng không để trống ID!");
            }
            PriceAdjustmentPlan priceAdjustmentPlan = await _unitOfWork.GetRepository<PriceAdjustmentPlan>()
                .GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy kết hoạch điều chỉnh giá phòng!");


            string userId = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);
            priceAdjustmentPlan = _mapper.Map<PriceAdjustmentPlan>(model);
            priceAdjustmentPlan.LastUpdatedBy = userId;
            priceAdjustmentPlan.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<PriceAdjustmentPlan>().UpdateAsync(priceAdjustmentPlan);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteRoomPriceAdjustmentPlan(string id)
        {
            if(String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID!");
            }    
            PriceAdjustmentPlan priceAdjustmentPlan =await _unitOfWork.GetRepository<PriceAdjustmentPlan>().GetByIdAsync(id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy kết hoạch điều chỉnh giá phòng!");
            string userId=Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            priceAdjustmentPlan.DeletedBy=userId;
            priceAdjustmentPlan.DeletedTime=CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<PriceAdjustmentPlan>().UpdateAsync(priceAdjustmentPlan);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
