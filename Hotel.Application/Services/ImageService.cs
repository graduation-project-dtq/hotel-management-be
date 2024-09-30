using Hotel.Application.DTOs.ImageDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Base;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Hotel.Application.Services
{
    public class ImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PaginatedList<GetImageDTO>> GetPageAsync(int index, int pageSize, string idSearch)
        {
            if (index <= 0 || pageSize <= 0)
            {
                throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.BADREQUEST, "Vui lòng nhập số trang hợp lệ!");
            }

            IQueryable<Image> query = _unitOfWork.GetRepository<Image>().Entities
                .Where(c => !c.DeletedTime.HasValue)
                .OrderByDescending(c => c.CreatedTime);

            if (!string.IsNullOrWhiteSpace(idSearch))
            {
                var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\-]+$");

                if (!regex.IsMatch(idSearch.Trim()))
                {
                    throw new ErrorException(StatusCodes.Status400BadRequest, ResponseCodeConstants.INVALID_INPUT, "ID không hợp lệ! Không được chứa ký tự đặc biệt.");
                }

                var ingredient = await query.FirstOrDefaultAsync(i => i.Id == idSearch);
                if (ingredient == null)
                {
                    throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Không tìm thấy ảnh với ID.");
                }

                query = query.Where(i => i.Id == idSearch);
            }

            // Ánh xạ từ Image sang GetImageDTO
            var imageDTOs = query.Select(img => new GetImageDTO
            {
                Id = img.Id,
                URL = img.URL,
                // Các thuộc tính khác...
            });

            // Sử dụng PaginatedList để phân trang
            return await PaginatedList<GetImageDTO>.CreateAsync(imageDTOs, index, pageSize);
        }
    }
}