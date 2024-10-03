using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.Base
{
    public class PaginatedList<T>
    {
        // Thuộc tính để lưu trữ các mục sau khi phân trang
        public IReadOnlyCollection<T> Items { get; }

        // Số trang hiện tại
        public int PageNumber { get; }

        // Tổng số trang
        public int TotalPages { get; }

        // Tổng số lượng mục
        public int TotalCount { get; }

        // Số mục trên mỗi trang
        public int PageSize { get; }

        // Constructor nhận vào danh sách các mục, số lượng mục, số trang và kích thước trang
        public PaginatedList(IReadOnlyCollection<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (count == 0) ? 1 : (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
            PageSize = pageSize;
        }

        // Kiểm tra xem có trang trước đó không
        public bool HasPreviousPage => PageNumber > 1;

        // Kiểm tra xem có trang tiếp theo không
        public bool HasNextPage => PageNumber < TotalPages;

        // Phương thức bất đồng bộ tạo danh sách phân trang từ IQueryable
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync(); // Đếm tổng số mục
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(); // Lấy các mục cho trang hiện tại

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }

        // Phương thức đồng bộ tạo danh sách phân trang từ List
        public static PaginatedList<T> Create(List<T> source, int pageNumber, int pageSize)
        {
            var totalCount = source.Count; // Đếm tổng số mục
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(); // Lấy các mục cho trang hiện tại

            return new PaginatedList<T>(items, totalCount, pageNumber, pageSize);
        }
    }
}
