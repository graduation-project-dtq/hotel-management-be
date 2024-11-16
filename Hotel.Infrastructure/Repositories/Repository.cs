using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Interfaces;
using Hotel.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet;
        protected readonly HotelDBContext _context;

        public IQueryable<T> Entities => _context.Set<T>();

        public Repository(HotelDBContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(string id)
        {
            T entity = await _dbSet.FindAsync(id) 
                ?? throw new ErrorException(StatusCodes.Status404NotFound , ResponseCodeConstants.NOT_FOUND,"Không tìn thấy");
            _dbSet.Remove(entity);
        }
        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities); // Sử dụng EF Core để cập nhật hàng loạt
            await Task.CompletedTask;
        }
        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities); // Xóa các thực thể trong DbSet
            await Task.CompletedTask; // Đảm bảo tính chất bất đồng bộ
        }

    }
}
