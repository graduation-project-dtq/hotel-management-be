using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
      
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Facilities> Facilities { get; set; }
        public DbSet<FacilitiesRoom> FacilitiesRooms { get; set; }
        public DbSet<Floor> Floors {  get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PriceAdjustmentPlan> PriceAdjustmentPlans { get; set; }
        public DbSet<Punish> Punishes { get; set; }
        public DbSet<Room>Rooms { get; set; }
        public DbSet<RoomPriceAdjustment> RoomPriceAdjustments { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<RoomTypeDetail> RoomTypeDetails { get; set; }
     
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceBooking> ServicesBooking { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherRoomTypeDetail>VoucherRoomTypeDetails { get; set; }
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

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Account) 
                .WithOne()
                .HasForeignKey<Customer>(c => c.AccountID); 

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.AccountID)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Account)
                .WithOne()
                .HasForeignKey<Employee>(e => e.AccountID);

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


            modelBuilder.Entity<Service>()
              .Property(s => s.Price)
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

            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code)
                .IsUnique();


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
            //FacilitiesRoom
            modelBuilder.Entity<FacilitiesRoom>()
                .HasKey(bk => new { bk.RoomID, bk.FacilitiesID });

            modelBuilder.Entity<FacilitiesRoom>()
                .HasOne(ft => ft.Room)
                .WithMany(f => f.FacilitiesRooms)
                .HasForeignKey(ft => ft.RoomID);

            modelBuilder.Entity<FacilitiesRoom>()
                .HasOne(ft => ft.Facilities)
                .WithMany(t => t.FacilitiesRooms)
                .HasForeignKey(ft => ft.FacilitiesID);

            //ImageEvaluation
            modelBuilder.Entity<ImageEvaluation>()
                .HasKey(bk => new { bk.ImageID, bk.EvaluationID });

            modelBuilder.Entity<ImageEvaluation>()
                .HasOne(ft => ft.Image)
                .WithMany(f => f.Evaluations)
                .HasForeignKey(ft => ft.ImageID);

            modelBuilder.Entity<ImageEvaluation>()
                .HasOne(ft => ft.Evaluation)
                .WithMany(t => t.ImageEvaluations)
                .HasForeignKey(ft => ft.EvaluationID);

            //ImageFacilities
            modelBuilder.Entity<ImageFacilities>()
                .HasKey(bk => new { bk.ImageID, bk.FacilitiesID });

            modelBuilder.Entity<ImageFacilities>()
                .HasOne(ft => ft.Image)
                .WithMany(f => f.ImageFacilities)
                .HasForeignKey(ft => ft.ImageID);

            modelBuilder.Entity<ImageFacilities>()
                .HasOne(ft => ft.Facilities)
                .WithMany(t => t.ImageFacilities)
                .HasForeignKey(ft => ft.FacilitiesID);

            //ImageRoomType
            modelBuilder.Entity<ImageRoomType>()
                .HasKey(bk => new { bk.ImageID, bk.RoomTypeID });

            modelBuilder.Entity<ImageRoomType>()
                .HasOne(ft => ft.Image)
                .WithMany(f => f.ImageRoomTypes)
                .HasForeignKey(ft => ft.ImageID);

            modelBuilder.Entity<ImageRoomType>()
                .HasOne(ft => ft.RoomType)
                .WithMany(t => t.ImageRoomTypes)
                .HasForeignKey(ft => ft.RoomTypeID);

            //ImageRoomTypeDetail
            modelBuilder.Entity<ImageRoomTypeDetail>()
                .HasKey(bk => new { bk.ImageID, bk.RoomTypeDetailID });

            modelBuilder.Entity<ImageRoomTypeDetail>()
                .HasOne(ft => ft.Image)
                .WithMany(f => f.ImageRoomTypesDetail)
                .HasForeignKey(ft => ft.ImageID);

            modelBuilder.Entity<ImageRoomTypeDetail>()
                .HasOne(ft => ft.RoomTypeDetail)
                .WithMany(t => t.ImageRoomTypeDetails)
                .HasForeignKey(ft => ft.RoomTypeDetailID);


            //ImageService
            modelBuilder.Entity<ImageService>()
                .HasKey(bk => new { bk.ImageID, bk.ServiceID });

            modelBuilder.Entity<ImageService>()
                .HasOne(ft => ft.Image)
                .WithMany(f => f.ImageServices)
                .HasForeignKey(ft => ft.ImageID);

            modelBuilder.Entity<ImageService>()
                .HasOne(ft => ft.Service)
                .WithMany(t => t.ImageServices)
                .HasForeignKey(ft => ft.ServiceID);

            //Punish
            modelBuilder.Entity<Punish>()
                .HasKey(bk => new { bk.BookingID, bk.FacilitiesID });

            modelBuilder.Entity<Punish>()
                .HasOne(ft => ft.Booking)
                .WithMany(f => f.Punishes)
                .HasForeignKey(ft => ft.BookingID);

            modelBuilder.Entity<Punish>()
                .HasOne(ft => ft.Facilities)
                .WithMany(t => t.Punishes)
                .HasForeignKey(ft => ft.FacilitiesID);

            //RoomPriceAdjustment
            modelBuilder.Entity<RoomPriceAdjustment>()
             .HasKey(bk => new { bk.RoomTypeDetailId, bk.PriceAdjustmentPlanId });

            modelBuilder.Entity<RoomPriceAdjustment>()
                .HasOne(ft => ft.RoomTypeDetail)
                .WithMany(ft => ft.RoomPriceAdjustments)
                .HasForeignKey(ft => ft.RoomTypeDetailId);

            modelBuilder.Entity<RoomPriceAdjustment>()
                .HasOne(ft => ft.PriceAdjustmentPlan)
                .WithMany(t => t.RoomPriceAdjustments)
                .HasForeignKey(ft => ft.PriceAdjustmentPlanId);

            //ServiceBooking
            modelBuilder.Entity<ServiceBooking>()
                .HasKey(bk => new { bk.ServiceID, bk.BookingID });

            modelBuilder.Entity<ServiceBooking>()
                .HasOne(ft => ft.Service)
                .WithMany(f => f.ServiceBookings)
                .HasForeignKey(ft => ft.ServiceID);

            modelBuilder.Entity<ServiceBooking>()
                .HasOne(ft => ft.Booking)
                .WithMany(t => t.ServiceBookings)
                .HasForeignKey(ft => ft.BookingID);

            //BookingDetail
            modelBuilder.Entity<BookingDetail>()
               .HasKey(bk => new { bk.BookingId, bk.RoomID });

            modelBuilder.Entity<BookingDetail>()
                .HasOne(ft => ft.Booking)
                .WithMany(f => f.BookingDetails)
                .HasForeignKey(ft => ft.BookingId);

            modelBuilder.Entity<BookingDetail>()
                .HasOne(ft => ft.Room)
                .WithMany(t => t.BookingDetails)
                .HasForeignKey(ft => ft.RoomID);

            //VoucherRoomTypeDetail
            modelBuilder.Entity<VoucherRoomTypeDetail>()
            .HasKey(bk => new { bk.VoucherID, bk.RoomTypeDetailID });

            modelBuilder.Entity<VoucherRoomTypeDetail>()
               .HasOne(ft => ft.Voucher)
               .WithMany(f => f.VoucherRoomTypeDetails)
               .HasForeignKey(ft => ft.VoucherID);

            modelBuilder.Entity<VoucherRoomTypeDetail>()
              .HasOne(ft => ft.RoomTypeDetail)
              .WithMany(f => f.VoucherRoomTypeDetails)
              .HasForeignKey(ft => ft.RoomTypeDetailID);
        }
    }
}
