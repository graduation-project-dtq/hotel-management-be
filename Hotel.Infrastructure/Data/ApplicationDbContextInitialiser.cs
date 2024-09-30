using Hotel.Core.Base;
using Hotel.Core.Common;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;


namespace Hotel.Infrastructure.Data
{
    public class ApplicationDbContextInitialiser
    {
        private readonly HotelDBContext _context;
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ApplicationDbContextInitialiser(
               HotelDBContext context,
               ILogger<ApplicationDbContextInitialiser> logger,
               IUnitOfWork unitOfWork)
        {
            _context = context;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    Boolean dbExist = await _context.Database.CanConnectAsync();
                    if (!dbExist)
                    {
                        await _context.Database.EnsureCreatedAsync();
                        await _context.Database.MigrateAsync();
                    }
                    else
                    {
                        await _context.Database.MigrateAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await addRole();
                await addAccount();
                await addRoomType();
                await addImage();
                await addImageRoomType();
                //await addUserRole();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task addRole()
        {
            if (!await _context.Roles.AnyAsync(x => x.DeletedTime == null))
            {
                Role[] roles =
                [
                    new Role {Id="11c1b04e29524abdbebd96ec80b6bc58", RoleName = CLAIMS_VALUES.ROLE_TYPE.ADMIN },
                    new Role {Id="51be69a4b2144a8987551569a468b064", RoleName = CLAIMS_VALUES.ROLE_TYPE.EMPLOYEE },
                    new Role {Id="c401bb08da484925900a63575c3717f8", RoleName = CLAIMS_VALUES.ROLE_TYPE.CUSTOMER },
                ];

                foreach (Role role in roles)
                {
                    if (!await _unitOfWork.GetRepository<Role>().Entities.AnyAsync(r => r.RoleName == role.RoleName))
                    {
                        role.CreatedTime = DateTime.Now;
                        role.LastUpdatedTime = DateTime.Now;
                        await _unitOfWork.GetRepository<Role>().InsertAsync(role);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }


        private async Task addAccount()
        {
            FixedSaltPasswordHasher<Account> passwordHasher = new FixedSaltPasswordHasher<Account>(Options.Create(new PasswordHasherOptions()));
            if (!await _context.Accounts.AnyAsync(x => x.DeletedTime == null))
            {
                var organizerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == CLAIMS_VALUES.ROLE_TYPE.ADMIN);
                if (organizerRole != null)
                {
                    var account = new Account
                    {
                        Id= "bce67c17cdd9476abd8644bd5abd47bf",
                        Email = "admin@gmail.com",
                        Password = passwordHasher.HashPassword(null, "Admin@123"),
                        RoleId = organizerRole.Id,
                        IsActive = true,
                        CreatedTime = DateTime.Now,
                        LastUpdatedTime = DateTime.Now
                    };

                    await _unitOfWork.GetRepository<Account>().InsertAsync(account);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }
        private async Task addRoomType()
        {
            RoomType[] roomTypes =
                [
                    new RoomType {Id="11c1b04e29524abdbebd96ec80d6bc58", Name = "Standard" ,Description="Standard,asfdbf,asff,awfqewf,qwfrwfew,qwrf"},
                    new RoomType {Id="51be69a4b2144a8987551569a428b064", Name = "Luxury" ,Description="Luxury"},
                    new RoomType {Id="c401bb08da484925900a63575c2717f8", Name = "Premium" ,Description="Premium"},
                ];
            foreach (var item in roomTypes)
            {
                if (!await _unitOfWork.GetRepository<RoomType>().Entities.AnyAsync(r => r.Name == item.Name))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<RoomType>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task addImage()
        {
            Image[] images =
            {
                //Stander
                new Image{Id="51be69a4b2144a8987551569a428bdfg",URL="https://cdn.kiwicollection.com/media/property/PR005988/xl/005988-06-Hudson-Studio.jpg?cb=1413923207"},
                new Image{Id="51be69a4b2144a8987551569a4282dwg",URL="https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRhggRRPHwX4uC9jF9KHmk6K5sW7OROwxbMdQ&s"},
                new Image{Id="51be69a4b2144a8987551523d428bdfg",URL="https://cdn.kiwicollection.com/media/property/PR005988/xl/005988-06-Hudson-Studio.jpg?cb=1413923207"},
                //Luxury
                new Image{Id="51be69a4b2144a898752ff69a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/590834367.jpg?k=67bf330a8c5dcb542e77be19eed8d607ef4523f34d1aa14357c4cc4179b56e73&o=&hp=1"},
                new Image{Id="51be69a4b2144a89875aef69a428bdfg",URL="https://media.gettyimages.com/id/1334117334/photo/digital-render-of-large-hotel-suite-bedroom.jpg?s=612x612&w=gi&k=20&c=PqNx8paT1x-Y3vmlQec3MCTWPkQiw6Oy4zkj_WrFVOs="},
                new Image{Id="51be69a4b2144a8987513d69a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/590832188.jpg?k=69307328db0257c93f40c9c116d2839f819b182de509541f059167ef9b1d23d7&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987523r69a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/590831949.jpg?k=5e8df42e84d4cc77633b5eb66389c26cb0156e92161bdcb89f0439676239f901&o=&hp=1"},
                //Premium
                new Image{Id="51be69a4b2144a8987524369a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/506732492.jpg?k=f03f26b3cdd0548108fe1a543c707a98edc26c2c1fdd1a9a196da1ee9ec3585c&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987sdff69a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/479057560.jpg?k=d666254dfb2a88a4688f41c8efb3ccf7b54e93c19f7b632b6e74dffbd5ab8f31&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987asff69a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/464243652.jpg?k=07d7d6774a8db017b51467fef5c6e71a4ad36b4ae11065f2e0d2b192b5095bc6&o=&hp=1"},
                new Image{Id="51be69a4b2144a89875saf69a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/464244742.jpg?k=599569893b6d6c7c4fff377db9ead1f08db79a2fe2bb135aa48848b2994d6491&o=&hp=1"},
                new Image{Id="51be69a4b214dfg987551569a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/479058972.jpg?k=21d19d812f5d61d56e2b9314f8e6de4a313bb1e04936b09275e8d52f2f2dae29&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428bdfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/479059384.jpg?k=bf60975ceea7dd208d2133889fbd6bfa708e4161f82998278866db2fb8b7c950&o=&hp=1"},
            };
            foreach(var item in images)
            {

                if (!await _unitOfWork.GetRepository<Image>().Entities.AnyAsync(i => i.URL == item.URL))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<Image>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task addImageRoomType()
        {
            ImageRoomType[] imageRoomTypes =
           {
                //Stadard
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551569a428bdfg"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551569a4282dwg"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bdfg"},
                //Luxury
                new ImageRoomType{RoomTypeID="51be69a4b2144a8987551569a428b064",ImageID="51be69a4b2144a898752ff69a428bdfg"},
                new ImageRoomType{RoomTypeID="51be69a4b2144a8987551569a428b064",ImageID="51be69a4b2144a8987sdff69a428bdfg"},
                new ImageRoomType{RoomTypeID="51be69a4b2144a8987551569a428b064",ImageID="51be69a4b2144a8987513d69a428bdfg"},
                new ImageRoomType{RoomTypeID="51be69a4b2144a8987551569a428b064",ImageID="51be69a4b2144a8987523r69a428bdfg"},
                //Premiun
                new ImageRoomType{RoomTypeID="c401bb08da484925900a63575c2717f8",ImageID="51be69a4b2144a8987524369a428bdfg"},
                new ImageRoomType{RoomTypeID="c401bb08da484925900a63575c2717f8",ImageID="51be69a4b2144a8987sdff69a428bdfg"},
                new ImageRoomType{RoomTypeID="c401bb08da484925900a63575c2717f8",ImageID="51be69a4b2144a8987asff69a428bdfg"},
                new ImageRoomType{RoomTypeID="c401bb08da484925900a63575c2717f8",ImageID="51be69a4b2144a89875saf69a428bdfg"},
                new ImageRoomType{RoomTypeID="c401bb08da484925900a63575c2717f8",ImageID="51be69a4b214dfg987551569a428bdfg"},
                new ImageRoomType{RoomTypeID="c401bb08da484925900a63575c2717f8",ImageID="51be69a4bdgf4a8987551569a428bdfg"},
            };
        }
    }
   
}
