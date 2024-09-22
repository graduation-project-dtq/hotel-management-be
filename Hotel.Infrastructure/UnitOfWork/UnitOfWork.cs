using Hotel.Domain.Interfaces;
using Hotel.Infrastructure.Data;
using Hotel.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.IOW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDBContext _context;
        private bool _disposed = false;

        public UnitOfWork(HotelDBContext context)
        {
            _context = context;
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
        }
    }
}
