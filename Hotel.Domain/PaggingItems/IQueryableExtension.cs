using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hotel.Infrastructure.Base;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Domain.PaggingItems
{
    public static class IQueryableExtension
    {
        // Phương thức phân trang
        public static Task<PaginatedList<T>> GetPaginatedList<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
        {
            return PaginatedList<T>.CreateAsync(source.AsNoTracking(), pageIndex, pageSize);
        }

        // Phương thức sử dụng ProjectTo để chuyển đổi dữ liệu sang kiểu TDestination và trả về danh sách
        public static Task<List<TDestination>> ProjectToListAsync<TDestination>(this IQueryable queryable, IConfigurationProvider configuration) where TDestination : class
        {
            return queryable.ProjectTo<TDestination>(configuration)
                            .AsNoTracking()
                            .ToListAsync();
        }
    }
}
