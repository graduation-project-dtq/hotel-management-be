using Hotel.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastructure.Data
{
    public class HotelDBContext : DbContext
    {
        public HotelDBContext()
        {

        }
        public HotelDBContext(DbContextOptions<HotelDBContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<CustomerVoucher> CustomerVouchers {  get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Facilities> Facilities { get; set; }
        public DbSet<FacilitiesRoom> FacilitiesRooms { get; set; }
        public DbSet<Floor> Floors {  get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PriceAdjustmentPlan> PriceAdjustmentPlans { get; set; }
        public DbSet<Punish> Punishes { get; set; }
        public DbSet<Room>Rooms { get; set; }
        public DbSet<RoomPrice> RoomPrices  { get; set; }
        public DbSet<RoomPriceAdjustment> RoomPriceAdjustments { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomTypeDetail> RoomTypeDetails { get; set; }
        public DbSet<RoomView> RoomViews    { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceBooking> ServicesBooking { get; set; }
        public DbSet<ViewHotel> ViewHotels { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<Image>  Images { get; set; }
        public DbSet<ImageEvaluation> ImageEvaluations { get; set; }
        public DbSet<ImageFacilities> ImageFacilities { get; set; }
        public DbSet<ImageRoomType> ImageRoomTypes { get; set; }
        public DbSet<ImageRoomTypeDetail> ImageRoomTypeDetails { get; set; }
        public DbSet<ImageService> ImageServices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(60),
                        errorNumbersToAdd: null);
                });
                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.AccountID)
                .IsUnique();


            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.AccountID)
                .IsUnique();

            modelBuilder.Entity<Booking>()
               .Property(b => b.TotalAmount)
               .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<BookingDetail>()
                .Property(bd => bd.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Facilities>()
                .Property(f => f.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RoomPrice>()
                .Property(r => r.BasePrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Service>()
              .Property(s => s.Price)
              .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ViewHotel>()
                    .Property(v => v.Price)
                    .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Punish>()
                  .Property(p => p.Fine)
                  .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PriceAdjustmentPlan>()
               .Property(f => f.AdjustmentValue)
               .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RoomTypeDetail>()
              .Property(f => f.Area)
              .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Voucher>()
             .Property(f => f.DiscountAmount)
             .HasColumnType("decimal(18, 2)");

            // Định nghĩa cho bảng Booking
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict); // hoặc DeleteBehavior.NoAction

            // Định nghĩa cho các ràng buộc khóa ngoại khác
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.RoomTypeDetail)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomTypeDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            // Định nghĩa cho bảng Punish
            modelBuilder.Entity<Punish>()
                .HasOne(p => p.Booking)
                .WithMany(b => b.Punishes)
                .HasForeignKey(p => p.BookingID)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Punish>()
                .HasOne(p => p.Facilities)
                .WithMany(f => f.Punishes)
                .HasForeignKey(p => p.FacilitiesID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
