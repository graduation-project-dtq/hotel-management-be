using Hotel.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XuongMay_BE.Contract.Repositories.Entities;


namespace Hotel.Repositories.Context
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "DbConnect",
                    options => options.MigrationsAssembly("Hotel_API"));
            }
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms{ get;set;}
    }
}
