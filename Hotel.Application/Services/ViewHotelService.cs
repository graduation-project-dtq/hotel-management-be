
using AutoMapper;
using Hotel.Application.DTOs.ViewHotelDTO;
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
    public class ViewHotelService : IViewHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        public ViewHotelService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }
        public async Task<PaginatedList<GetViewHotelDTO>> GetPageAsync(int index, int pageSize, string idSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Room> query = _unitOfWork.GetRepository<Room>().Entities
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
            var totalCount = await query.CountAsync();  // Tổng số bản ghi
            if (totalCount == 0)
            {
                return new PaginatedList<GetViewHotelDTO>(new List<GetViewHotelDTO>(), totalCount, index, pageSize);
            }

            var resultQuery = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();

            // Ánh xạ từ Room sang GetRoomDTO
            var responseItems = resultQuery.Select(room => _mapper.Map<GetViewHotelDTO>(room)).ToList();

            // Tạo danh sách phân trang
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var responsePaginatedList = new PaginatedList<GetViewHotelDTO>(
                responseItems,
                totalCount,
                index,
                pageSize
            );
            return responsePaginatedList;
        }

        public async Task<GetViewHotelDTO> CreateViewHotel(PostViewHotelDTO model)
        {
            if(String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.INVALID_INPUT, "Không được để trống tên!");
            }

            if(model.Price < 0 || model.Price % 1 != 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Giá phải là một số nguyên dương.");
            }

            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            ViewHotel viewHotel = _mapper.Map<ViewHotel>(model);

            viewHotel.CreatedBy=userID;
            viewHotel.LastUpdatedBy=userID;
            viewHotel.CreatedTime = CoreHelper.SystemTimeNow;
            viewHotel.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<ViewHotel>().InsertAsync(viewHotel);
            await _unitOfWork.SaveChangesAsync();

            GetViewHotelDTO getViewHotel = _mapper.Map<GetViewHotelDTO>(viewHotel);

            return getViewHotel;
        }

        public async Task<GetViewHotelDTO> UpdateViewHotel(string id,PutViewHotelDTO model)
        {
            if(String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID!");

            }
            ViewHotel view=await _unitOfWork.GetRepository<ViewHotel>().GetByIdAsync(id);

            if (view == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tồn tại view với ID nhập vào!");

            }
            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.INVALID_INPUT, "Không được để trống tên!");
            }

            if (model.Price < 0 || model.Price % 1 != 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "Giá phải là một số nguyên dương.");
            }


            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            view = _mapper.Map<ViewHotel>(model);


            view.LastUpdatedBy = userID;
            view.LastUpdatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.GetRepository<ViewHotel>().UpdateAsync(view); 
            await _unitOfWork.SaveChangesAsync();

            GetViewHotelDTO getViewHotel = _mapper.Map<GetViewHotelDTO>(view);

            return getViewHotel;
        }

        public async Task DeleteViewHotel(string id)
        {
            if(String.IsNullOrWhiteSpace(id))
            {
                throw new ErrorException(StatusCodes.Status406NotAcceptable, ResponseCodeConstants.INVALID_INPUT, "Không được để trống ID!");
            }
            ViewHotel view=await _unitOfWork.GetRepository<ViewHotel>().Entities.FirstOrDefaultAsync(v=>v.Id == id)
                ?? throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tồn tại view với ID");

            string userID = Authentication.GetUserIdFromHttpContextAccessor(_contextAccessor);

            view.DeletedBy = userID;
            view.DeletedTime=CoreHelper.SystemTimeNow;
            await _unitOfWork.GetRepository<ViewHotel>().UpdateAsync(view);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
