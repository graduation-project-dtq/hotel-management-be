using Hotel.Contract.Repositories.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Security;



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
        //public DbSet<User> Users { get; set; }
        //public DbSet<Room> Rooms { get; set; }

        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillDetail> BillDetails { get; set; }
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
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Bill>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Bill>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Bill>()
        //        .Property(e => e.EmployeeId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Bill>()
        //        .Property(e => e.BookingId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Bill>()
        //        .Property(e => e.TotalAmount)
        //        .HasPrecision(12, 2);

        //    modelBuilder.Entity<BillDetail>()
        //        .Property(e => e.BillId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<BillDetail>()
        //        .Property(e => e.ServiceId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Booking>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Booking>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Booking>()
        //        .Property(e => e.EmployeeId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Booking>()
        //        .Property(e => e.CustomerId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Booking>()
        //        .Property(e => e.RoomTypeDetailId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Booking>()
        //        .Property(e => e.TotalAmount)
        //        .HasPrecision(12, 2);

        //    modelBuilder.Entity<BookingDetail>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<BookingDetail>()
        //        .Property(e => e.BookingId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<BookingDetail>()
        //        .Property(e => e.RoomId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<BookingDetail>()
        //        .Property(e => e.Price)
        //        .HasPrecision(12, 2);

        //    modelBuilder.Entity<Customer>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Customer>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Customer>()
        //        .Property(e => e.Phone)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Customer>()
        //        .Property(e => e.Email)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Customer>()
        //     .HasMany(e => e.CustomerVouchers)
        //     .WithOne(e => e.Customer)
        //     .HasForeignKey(e => e.CustomerId)
        //     .OnDelete(DeleteBehavior.Restrict);

        //    modelBuilder.Entity<Customer>()
        //     .HasMany(e => e.Favorites)
        //     .WithOne(e => e.Customer)  // Sử dụng WithOne để biểu thị quan hệ một-một hoặc một-nhiều.
        //     .HasForeignKey(e => e.RoomTypeDetailId)
        //     .IsRequired(false); // Đặt IsRequired(false) để làm cho khóa ngoại là tùy chọn (optional)


        //    modelBuilder.Entity<CustomerVoucher>()
        //        .Property(e => e.CustomerId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<CustomerVoucher>()
        //        .Property(e => e.VoucherId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.IdentityCard)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.Phone)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Employee>()
        //        .Property(e => e.Email)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Evaluation>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Evaluation>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Evaluation>()
        //        .Property(e => e.CustomerId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Evaluation>()
        //        .Property(e => e.RoomTypeDetailId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Favorite>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Favorite>()
        //        .Property(e => e.RoomTypeDetailId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Favorite>()
        //        .Property(e => e.CustomerId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Floor>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);


        //    modelBuilder.Entity<PriceAdjustmentPlan>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<PriceAdjustmentPlan>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<PriceAdjustmentPlan>()
        //        .Property(e => e.AdjustmentValue)
        //        .HasPrecision(12, 2);


        //    modelBuilder.Entity<Room>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Room>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Room>()
        //        .Property(e => e.FloorId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Room>()
        //        .Property(e => e.RoomTypeDetailId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomCategory>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomCategory>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomPrice>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomPrice>()
        //        .Property(e => e.RoomTypeDetailId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomPrice>()
        //        .Property(e => e.BasePrice)
        //        .HasPrecision(12, 2);

        //    modelBuilder.Entity<RoomPriceAdjustment>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomPriceAdjustment>()
        //        .Property(e => e.RoomPriceId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomPriceAdjustment>()
        //        .Property(e => e.PriceAdjustmentPlanId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomTypeDetail>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomTypeDetail>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomTypeDetail>()
        //        .Property(e => e.RoomCategoryId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<RoomTypeDetail>()
        //        .Property(e => e.Area)
        //        .HasPrecision(12, 2);



        //    modelBuilder.Entity<Service>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Service>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Service>()
        //        .Property(e => e.Price)
        //        .HasPrecision(12, 2);

        //    modelBuilder.Entity<Service>()
        //        .Property(e => e.Image)
        //        .IsUnicode(false);



        //    modelBuilder.Entity<User>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<User>()
        //        .Property(e => e.EmployeeId)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<User>()
        //        .Property(e => e.CustomerId)
        //        .IsUnicode(false);



        //    modelBuilder.Entity<Voucher>()
        //        .Property(e => e.Id)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Voucher>()
        //        .Property(e => e.InternalCode)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Voucher>()
        //        .Property(e => e.Code)
        //        .IsUnicode(false);

        //    modelBuilder.Entity<Voucher>()
        //        .Property(e => e.DiscountAmount)
        //        .HasPrecision(12, 2);

        //    modelBuilder.Entity<Voucher>()
        //      .HasMany(e => e.CustomerVouchers)
        //      .WithOne(e => e.Voucher)
        //      .HasForeignKey(e => e.VoucherId) // Chỉ định khóa ngoại
        //      .OnDelete(DeleteBehavior.Restrict); // Ngăn chặn cascade delete
        //    ///Khai báo khoá chính có nhiều thuộc tính
        //    modelBuilder.Entity<BillDetail>()
        //       .HasKey(b => new { b.BillId, b.ServiceId }); // Khóa chính kết hợp
        //    modelBuilder.Entity<BookingDetail>()
        //     .HasKey(b => new { b.BookingId, b.RoomId }); // Khóa chính kết hợp
        //                                                  // Cấu hình khóa chính cho IdentityUserLogin
        //    modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(l => new { l.LoginProvider, l.ProviderKey });

        //    // Cấu hình khóa chính cho IdentityUserRole
        //    modelBuilder.Entity<IdentityUserRole<string>>().HasKey(r => new { r.UserId, r.RoleId });

        //    // Cấu hình khóa chính cho IdentityUserToken
        //    modelBuilder.Entity<IdentityUserToken<string>>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình cho bảng Bill
            modelBuilder.Entity<Bill>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Bill>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Bill>()
                .Property(e => e.EmployeeId)
                .IsUnicode(false);

            modelBuilder.Entity<Bill>()
                .Property(e => e.BookingId)
                .IsUnicode(false);

            modelBuilder.Entity<Bill>()
                .Property(e => e.TotalAmount)
                .HasPrecision(12, 2);

            // Cấu hình cho bảng BillDetail
            modelBuilder.Entity<BillDetail>()
                .Property(e => e.BillId)
                .IsUnicode(false);

            modelBuilder.Entity<BillDetail>()
                .Property(e => e.ServiceId)
                .IsUnicode(false);

            // Khóa chính kết hợp cho bảng BillDetail
            modelBuilder.Entity<BillDetail>()
                .HasKey(b => new { b.BillId, b.ServiceId });

            // Cấu hình cho bảng Booking
            modelBuilder.Entity<Booking>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.EmployeeId)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.CustomerId)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.RoomTypeDetailId)
                .IsUnicode(false);

            modelBuilder.Entity<Booking>()
                .Property(e => e.TotalAmount)
                .HasPrecision(12, 2);

            // Cấu hình cho bảng BookingDetail
            modelBuilder.Entity<BookingDetail>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<BookingDetail>()
                .Property(e => e.BookingId)
                .IsUnicode(false);

            modelBuilder.Entity<BookingDetail>()
                .Property(e => e.RoomId)
                .IsUnicode(false);

            modelBuilder.Entity<BookingDetail>()
                .Property(e => e.Price)
                .HasPrecision(12, 2);

            // Khóa chính kết hợp cho bảng BookingDetail
            modelBuilder.Entity<BookingDetail>()
                .HasKey(b => new { b.BookingId, b.RoomId });

            // Cấu hình khóa ngoại BookingDetail - Room
            modelBuilder.Entity<BookingDetail>()
                .HasOne(b => b.Room)
                .WithMany(r => r.BookingDetails)
                .HasForeignKey(b => b.RoomId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh sử dụng cascade delete để tránh xung đột

            // Cấu hình khóa ngoại BookingDetail - Booking
            modelBuilder.Entity<BookingDetail>()
                .HasOne(b => b.Booking)
                .WithMany(bk => bk.BookingDetails)
                .HasForeignKey(b => b.BookingId)
                .OnDelete(DeleteBehavior.Cascade); // Chỉ dùng cascade delete với một khóa ngoại

            // Cấu hình cho bảng Customer
            modelBuilder.Entity<Customer>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.CustomerVouchers)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
                .HasMany(e => e.Favorites)
                .WithOne(e => e.Customer)
                .HasForeignKey(e => e.RoomTypeDetailId)
                .IsRequired(false);

            // Cấu hình cho bảng CustomerVoucher
            modelBuilder.Entity<CustomerVoucher>()
                .Property(e => e.CustomerId)
                .IsUnicode(false);

            modelBuilder.Entity<CustomerVoucher>()
                .Property(e => e.VoucherId)
                .IsUnicode(false);

            // Cấu hình cho bảng Employee
            modelBuilder.Entity<Employee>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.IdentityCard)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Phone)
                .IsUnicode(false);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Email)
                .IsUnicode(false);

            // Cấu hình cho bảng Evaluation
            modelBuilder.Entity<Evaluation>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.CustomerId)
                .IsUnicode(false);

            modelBuilder.Entity<Evaluation>()
                .Property(e => e.RoomTypeDetailId)
                .IsUnicode(false);

            // Cấu hình cho bảng Favorite
            modelBuilder.Entity<Favorite>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Favorite>()
                .Property(e => e.RoomTypeDetailId)
                .IsUnicode(false);

            modelBuilder.Entity<Favorite>()
                .Property(e => e.CustomerId)
                .IsUnicode(false);

            // Cấu hình cho bảng Floor
            modelBuilder.Entity<Floor>()
                .Property(e => e.Id)
                .IsUnicode(false);

            // Cấu hình cho bảng PriceAdjustmentPlan
            modelBuilder.Entity<PriceAdjustmentPlan>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<PriceAdjustmentPlan>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<PriceAdjustmentPlan>()
                .Property(e => e.AdjustmentValue)
                .HasPrecision(12, 2);

            // Cấu hình cho bảng Room
            modelBuilder.Entity<Room>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.FloorId)
                .IsUnicode(false);

            modelBuilder.Entity<Room>()
                .Property(e => e.RoomTypeDetailId)
                .IsUnicode(false);

            // Cấu hình cho bảng RoomCategory
            modelBuilder.Entity<RoomCategory>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<RoomCategory>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            // Cấu hình cho bảng RoomPrice
            modelBuilder.Entity<RoomPrice>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<RoomPrice>()
                .Property(e => e.RoomTypeDetailId)
                .IsUnicode(false);

            modelBuilder.Entity<RoomPrice>()
                .Property(e => e.BasePrice)
                .HasPrecision(12, 2);

            // Cấu hình cho bảng RoomPriceAdjustment
            modelBuilder.Entity<RoomPriceAdjustment>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<RoomPriceAdjustment>()
                .Property(e => e.RoomPriceId)
                .IsUnicode(false);

            modelBuilder.Entity<RoomPriceAdjustment>()
                .Property(e => e.PriceAdjustmentPlanId)
                .IsUnicode(false);

            // Cấu hình cho bảng RoomTypeDetail
            modelBuilder.Entity<RoomTypeDetail>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<RoomTypeDetail>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<RoomTypeDetail>()
                .Property(e => e.RoomCategoryId)
                .IsUnicode(false);

            modelBuilder.Entity<RoomTypeDetail>()
                .Property(e => e.Area)
                .HasPrecision(12, 2);

            // Cấu hình cho bảng Service
            modelBuilder.Entity<Service>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Service>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Service>()
                .Property(e => e.Price)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Service>()
                .Property(e => e.Image)
                .IsUnicode(false);

            // Cấu hình cho bảng User
            modelBuilder.Entity<User>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.EmployeeId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.CustomerId)
                .IsUnicode(false);
            modelBuilder.Entity<User>()
               .Property(u => u.Image)
               .IsRequired(false); 
            // Cấu hình cho bảng Voucher
            modelBuilder.Entity<Voucher>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Voucher>()
                .Property(e => e.InternalCode)
                .IsUnicode(false);

            modelBuilder.Entity<Voucher>()
                .Property(e => e.Code)
                .IsUnicode(false);

            modelBuilder.Entity<Voucher>()
                .Property(e => e.DiscountAmount)
                .HasPrecision(12, 2);

            // Cấu hình khóa chính cho IdentityUserLogin
            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });

            // Cấu hình khóa chính cho IdentityUserRole
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new { r.UserId, r.RoleId });

            // Cấu hình khóa chính cho IdentityUserToken
            modelBuilder.Entity<IdentityUserToken<string>>()
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            base.OnModelCreating(modelBuilder);
        }

    }
}
