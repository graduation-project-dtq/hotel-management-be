using Hotel.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Repositories.Context
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        // Define DbSet properties for all your entities
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingDetail> BookingDetails { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerVoucher> CustomerVouchers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Evaluation> Evaluations { get; set; }
        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Floor> Floors { get; set; }
        public virtual DbSet<PriceAdjustmentPlan> PriceAdjustmentPlans { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomCategory> RoomCategories { get; set; }
        public virtual DbSet<RoomPrice> RoomPrices { get; set; }
        public virtual DbSet<RoomPriceAdjustment> RoomPriceAdjustments { get; set; }
        public virtual DbSet<RoomTypeDetail> RoomTypeDetails { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<Facilities> Facilities { get; set; }
        public virtual DbSet<FacilitiesRoom> FacilitiesRooms { get; set; }
        public virtual DbSet<ServiceBooking> ServiceBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Cấu hình các thực thể khác
           
            // Cấu hình mối quan hệ và khóa ngoại của Identity
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityUser<string>>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole<string>>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

          
            modelBuilder.Entity<IdentityUserClaim<string>>()
                .HasOne<IdentityUser<string>>()
                .WithMany()
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

          
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasOne<IdentityUser<string>>()
                .WithMany()
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasOne<IdentityUser<string>>()
                .WithMany()
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình các thực thể khác
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Employee)
                .WithMany(e => e.Users)
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Customer)
                .WithMany(c => c.Bookings)
                .HasForeignKey(b => b.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.RoomTypeDetail)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.RoomTypeDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingDetail>()
                .HasKey(bd => new { bd.BookingId, bd.RoomId });

            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Booking)
                .WithMany(b => b.BookingDetails)
                .HasForeignKey(bd => bd.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Room)
                .WithMany(r => r.BookingDetails)
                .HasForeignKey(bd => bd.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerVoucher>()
                .HasKey(cv => new { cv.CustomerId, cv.VoucherId });

            modelBuilder.Entity<CustomerVoucher>()
                .HasOne(cv => cv.Customer)
                .WithMany(c => c.CustomerVouchers)
                .HasForeignKey(cv => cv.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerVoucher>()
                .HasOne(cv => cv.Voucher)
                .WithMany(v => v.CustomerVouchers)
                .HasForeignKey(cv => cv.VoucherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Customer)
                .WithMany(c => c.Evaluations)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.RoomTypeDetail)
                .WithMany(r => r.Evaluations)
                .HasForeignKey(e => e.RoomTypeDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Customer)
                .WithMany(c => c.Favorites)
                .HasForeignKey(f => f.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.RoomTypeDetail)
                .WithMany(r => r.Favorites)
                .HasForeignKey(f => f.RoomTypeDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Floor)
                .WithMany(f => f.Rooms)
                .HasForeignKey(r => r.FloorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomTypeDetail)
                .WithMany(rtd => rtd.Rooms)
                .HasForeignKey(r => r.RoomTypeDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomPrice>()
                .HasOne(rp => rp.RoomTypeDetail)
                .WithMany(rtd => rtd.RoomPrices)
                .HasForeignKey(rp => rp.RoomTypeDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomPriceAdjustment>()
                .HasOne(rpa => rpa.RoomPrice)
                .WithMany(rp => rp.RoomPriceAdjustments)
                .HasForeignKey(rpa => rpa.RoomPriceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomPriceAdjustment>()
                .HasOne(rpa => rpa.PriceAdjustmentPlan)
                .WithMany(pap => pap.RoomPriceAdjustments)
                .HasForeignKey(rpa => rpa.PriceAdjustmentPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomTypeDetail>()
                .HasOne(rtd => rtd.RoomCategory)
                .WithMany(rc => rc.RoomTypeDetails)
                .HasForeignKey(rtd => rtd.RoomCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceBooking>()
                .HasKey(sb => new { sb.ServiceID, sb.BookingID });

            modelBuilder.Entity<ServiceBooking>()
                .HasOne(sb => sb.Booking)
                .WithMany(b => b.ServiceBookings)
                .HasForeignKey(sb => sb.BookingID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceBooking>()
                .HasOne(sb => sb.Service)
                .WithMany(s => s.ServiceBookings)
                .HasForeignKey(sb => sb.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacilitiesRoom>()
                .HasKey(fr => new { fr.FacilitiesID, fr.RoomID });

            modelBuilder.Entity<FacilitiesRoom>()
                .HasOne(fr => fr.Facilities)
                .WithMany(f => f.FacilitiesRooms)
                .HasForeignKey(fr => fr.FacilitiesID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacilitiesRoom>()
                .HasOne(fr => fr.Room)
                .WithMany(r => r.FacilitiesRooms)
                .HasForeignKey(fr => fr.RoomID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomView>()
               .HasKey(ur => new { ur.RoomId, ur.ViewHotelId });

            modelBuilder.Entity<RoomView>()
              .HasKey(ur => new { ur.RoomId, ur.ViewHotelId });
        }
    }
}
