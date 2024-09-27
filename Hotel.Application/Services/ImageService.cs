﻿using Hotel.Application.DTOs.ImageDTO; // Kiểm tra namespace
using Hotel.Application.Interfaces;
using Hotel.Core.Base; // Đảm bảo có namespace đúng
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

            // Kiểm tra id
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

            // Tạo danh sách phân trang
            var imageDTOs = query.Select(img => new GetImageDTO // Ánh xạ từ Image sang GetImageDTO
            {
                Id = img.Id,
                URL = img.URL,
                // Thêm các thuộc tính cần thiết khác
            });

            return await PaginatedList<GetImageDTO>.CreateAsync(imageDTOs, index, pageSize);
        }

    }
}
