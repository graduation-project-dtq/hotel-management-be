﻿using Hotel.Core.Base;
using Hotel.Core.Common;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums.EnumRoom;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


using Image = Hotel.Domain.Entities.Image;


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
                await addRoomPrice();
                await addRoomTypeDetail();
                await addFloor();
                await addHouse();
                await addRoom();
                await addFacilities();
                await addFacilitiesRoom();
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
                    new RoomType {Id="11c1b04e29524abdbebd96ec80d6bc58", Name = "Standard" ,
                        Description="Phòng Standard của chúng tôi mang đến một không gian thoải mái và ấm cúng," +
                        " hoàn hảo cho cả khách công tác và du lịch. Với bố trí tiện nghi," +
                        " phòng được trang bị một giường đôi cỡ queen hoặc hai giường đơn," +
                        " đảm bảo giấc ngủ ngon. Nội thất hiện đại tạo nên không khí ấm áp," +
                        " kèm theo các tiện nghi cần thiết như TV màn hình phẳng," +
                        " Wi-Fi miễn phí và khu vực làm việc riêng." +
                        "Phòng tắm riêng có các vật dụng toiletry miễn phí và vòi sen thư giãn," +
                        " mang lại trải nghiệm dễ chịu. Khách cũng có thể tận hưởng thêm các tiện " +
                        "ích như tủ lạnh mini và thiết bị pha cà phê, giúp nâng cao trải nghiệm lưu trú." +
                        " Với sự kết hợp giữa sự thoải mái và chức năng, " +
                        "phòng Standard của chúng tôi là lựa chọn lý tưởng cho bất kỳ ai muốn có một kỳ nghỉ thư giãn " +
                        "hoặc chuyến công tác hiệu quả."},
                    new RoomType {Id="51be69a4b2144a8987551569a428b064", Name = "Luxury" ,
                        Description="Hãy tận hưởng sự thoải mái và tinh tế tuyệt đối trong phòng Luxury của chúng tôi," +
                        " được thiết kế cho những khách hàng tinh tế tìm kiếm tiêu chuẩn sang trọng hơn." +
                        " Phòng rộng rãi này có một giường cỡ king được trang trí bằng ga trải giường chất lượng cao," +
                        " đảm bảo giấc ngủ ngon và phục hồi. Nội thất sang trọng và trang trí tinh tế tạo nên không khí yên tĩnh," +
                        " trong khi cửa sổ lớn mang đến tầm nhìn tuyệt đẹp ra khu vực xung quanh." +
                        "Phòng Luxury được trang bị các tiện nghi hiện đại," +
                        " bao gồm TV màn hình phẳng, Wi-Fi tốc độ cao và khu vực ngồi thoải mái, " +
                        "hoàn hảo cho việc thư giãn hoặc làm việc. " +
                        "Phòng tắm riêng sang trọng có bồn tắm ngâm, " +
                        "vòi sen riêng và các sản phẩm toiletry cao cấp để nâng cao trải nghiệm chăm sóc bản thân." +
                        " Hãy thưởng thức sự tiện lợi từ minibar và đồ uống miễn phí," +
                        " đảm bảo mỗi khoảnh khắc trong kỳ nghỉ của bạn là không thể quên."},
                    new RoomType {Id="c401bb08da484925900a63575c2717f8", Name = "Premium" ,
                        Description="Trải nghiệm sự thoải mái vượt trội trong phòng Premium của chúng tôi," +
                        " được thiết kế chu đáo cho cả sự thư giãn và năng suất. " +
                        "Phòng rộng rãi này có một giường đôi cỡ queen hoặc hai giường đơn, " +
                        "được trang trí bằng ga trải giường sang trọng để mang đến giấc ngủ ngon." +
                        " Thiết kế hiện đại và nội thất mời gọi tạo nên không khí ấm cúng," +
                        " kèm theo các tiện nghi cần thiết như TV màn hình phẳng," +
                        " Wi-Fi tốc độ cao và khu vực làm việc riêng." +
                        "nPhòng tắm riêng có các thiết bị hiện đại và sản phẩm toiletry cao cấp, " +
                        "đảm bảo trải nghiệm thoải mái và dễ chịu. Khách cũng sẽ tận hưởng thêm sự tiện lợi" +
                        " như tủ lạnh mini và thiết bị pha cà phê, hoàn hảo cho việc thư giãn sau một ngày dài." +
                        " Phòng Premium là lựa chọn lý tưởng cho những du khách mong muốn sự kết hợp giữa sự thoải mái " +
                        "và phong cách trong kỳ nghỉ của họ."},
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
            foreach (var item in imageRoomTypes)
            {

                if (!await _unitOfWork.GetRepository<ImageRoomType>().Entities.AnyAsync(i => i.RoomTypeID == item.RoomTypeID && i.ImageID==item.ImageID))
                {
                    await _unitOfWork.GetRepository<ImageRoomType>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task addRoomPrice()
        {
            RoomPrice[] roomPrices =
            {
                //Bảng giá cho Standard
                new RoomPrice(){Id="c401bb0823284925900a63575c2717f8",BasePrice=500000,Description="",Name="Giá phòng Standard 1 giường ngủ"},
                new RoomPrice(){Id="c401bbdt23c84925900a63575c2717f8",BasePrice=700000,Description="",Name="Giá phòng Standard 2 giường ngủ"},
                new RoomPrice(){Id="c401bb08da484ffff00a63575c2717f8",BasePrice=1000000,Description="",Name="Giá phòng Standard 3 giường ngủ"},

                //Bảng giá cho Luxury
                new RoomPrice(){Id="c401bb08dahgh925900a63575c2717f8",BasePrice=650000,Description="",Name="Giá phòng Luxury 1 giường ngủ"},
                new RoomPrice(){Id="c401bb08da484924400a63575c2717f8",BasePrice=800000,Description="",Name="Giá phòng Luxury 2 giường ngủ"},
                new RoomPrice(){Id="c401bb08da4849dgafftg3575c2717f8",BasePrice=1200000,Description="",Name="Giá phòng Luxury 3 giường ngủ"},

                //Bảng giá cho Premiun
                new RoomPrice(){Id="c401bb08da4842rffcas63575c2717f8",BasePrice=800000,Description="",Name="Giá phòng Premiun 1 giường ngủ"},
                new RoomPrice(){Id="c401bb08da4849dgf1fs63575c2717f8",BasePrice=1100000,Description="",Name="Giá phòng Premiun 2 giường ngủ"},
                new RoomPrice(){Id="c401bb08da48493erfffsv575c2717f8",BasePrice=1400000,Description="",Name="Giá phòng Premiun 3 giường ngủ"},
            };
            foreach (var item in roomPrices)
            {

                if (!await _unitOfWork.GetRepository<RoomPrice>().Entities.AnyAsync(i => i.Id == item.Id))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<RoomPrice>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task addRoomTypeDetail()
        {
            RoomTypeDetail[] roomTypeDetails =
            {
                //Standard
                new RoomTypeDetail(){Id="c401bb08da484fdgdggfsv575c2717f8",Name="Standard 1 giường ngủ",RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",RoomPriceID="c401bb0823284925900a63575c2717f8",CapacityMax=2,Area=50,AverageStart=0,Description=""},
                new RoomTypeDetail(){Id="c401bb08dfggggggrfffsv575c2717f8",Name="Standard 2 giường ngủ",RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",RoomPriceID="c401bbdt23c84925900a63575c2717f8",CapacityMax=4,Area=64,AverageStart=0,Description=""},
                new RoomTypeDetail(){Id="c401bb08da48493erfffsv575c2717f9",Name="Standard 3 giường ngủ",RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",RoomPriceID="c401bb08da484ffff00a63575c2717f8",CapacityMax=6,Area=70,AverageStart=0,Description=""},

                //Luxury
                new RoomTypeDetail(){Id="c401bb08dasgsdgdsggdgggg5c2717f8",Name="Luxury 1 giường ngủ",RoomTypeID="51be69a4b2144a8987551569a428b064",RoomPriceID="c401bb08dahgh925900a63575c2717f8",CapacityMax=2,Area=55,AverageStart=0,Description=""},
                new RoomTypeDetail(){Id="c401bb08dasdgswfsvwawfffhs271710",Name="Luxury 2 giường ngủ",RoomTypeID="51be69a4b2144a8987551569a428b064",RoomPriceID="c401bb08da484924400a63575c2717f8",CapacityMax=4,Area=66,AverageStart=0,Description=""},
                new RoomTypeDetail(){Id="c401bb08dfdgggggggffsv575c2717f8",Name="Luxury 3 giường ngủ",RoomTypeID="51be69a4b2144a8987551569a428b064",RoomPriceID="c401bb08da4849dgafftg3575c2717f8",CapacityMax=6,Area=74,AverageStart=0,Description=""},

                //Premium
                new RoomTypeDetail(){Id="c401bb08da4sddggggggfv575c271710",Name="Premium 1 giường ngủ",RoomTypeID="c401bb08da484925900a63575c2717f8",RoomPriceID="c401bb08da4842rffcas63575c2717f8",CapacityMax=2,Area=60,AverageStart=0,Description=""},
                new RoomTypeDetail(){Id="c401bb08da48493erdffsv575c271711",Name="Premium 2 giường ngủ",RoomTypeID="c401bb08da484925900a63575c2717f8",RoomPriceID="c401bb08da4849dgf1fs63575c2717f8",CapacityMax=4,Area=70,AverageStart=0,Description=""},
                new RoomTypeDetail(){Id="c401bb08da4849sdg35gggg75c271712",Name="Premium 3 giường ngủ",RoomTypeID="c401bb08da484925900a63575c2717f8",RoomPriceID="c401bb08da48493erfffsv575c2717f8",CapacityMax=6,Area=80,AverageStart=0,Description=""},
            };
            foreach (var item in roomTypeDetails)
            {

                if (!await _unitOfWork.GetRepository<RoomTypeDetail>().Entities.AnyAsync(i => i.Id == item.Id))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<RoomTypeDetail>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task addFloor()
        {
            Floor[] floors =
            {
                new Floor(){Id="c401bb08da4849sdg35ggggdfffffff2",Name="Toà A"},
                new Floor(){Id="c401bb08da4849sd346tgbdfgc271712",Name="Toà B"},
                new Floor(){Id="c401bb08da4849sdg35gggg75c271712",Name="Toà C"},
                new Floor(){Id="c401bb08da4849dfhdfffs3e5c271712",Name="Toà D"},
            };
            foreach (var item in floors)
            {

                if (!await _unitOfWork.GetRepository<Floor>().Entities.AnyAsync(i => i.Id == item.Id))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<Floor>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task addHouse()
        {

            HouseType[] floors =
            {
                new HouseType(){Id="c401bb08da4849sdg35ggggdfffffff2",Name="Phòng gia đình",Description="phòng gia đình"},
                new HouseType(){Id="c401bb08da4849sd346tgbdfgc271712",Name="Studio",Description="studio xịn xò con bò"},
                new HouseType(){Id="c401bb08da4849sdg35gggg75c271712",Name="Duplex",Description="không biết"},
            };
            foreach (var item in floors)
            {

                if (!await _unitOfWork.GetRepository<HouseType>().Entities.AnyAsync(i => i.Id == item.Id))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<HouseType>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task addRoom()
        {
            Room[] rooms =
            {
                // Toà A Standard
                new Room(){Id="c401bbdffa4849sdg35gggdd1ffffff2",Name="A101",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bb08da4849sdg35gggdd2ffffff2",Name="A102",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="ffffbb08da4849sdg35gggdd3ffffff2",Name="A103",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c4fghdf8da4849sdg35gggdd4ffffff2",Name="A104",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bbdfggggfdbbefffsgg5ff4ffff2",Name="A105",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bb08da4849sdg35gggdd6ffffff2",Name="A106",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="ffffbb08da4849sdg35gggdd7ffffff2",Name="A107",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c4fghdf8da4849sdg35gggdd8ffffff2",Name="A108",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bbdffa4849sdg35gggdd9ffffff2",Name="A201",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bb08da4849sdg35gggdd10fffff2",Name="A202",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"}, //
                new Room(){Id="ffffbbfggg4849sdg35gggdd11fffff2",Name="A203",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c4fghdf8da4849sdg35gggdd12fffff2",Name="A204",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bbdffa4849sdg35gggdd13fffff2",Name="A205",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35gggdd14fffff2",Name="A206",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="ffffbb08da4849sdg35gggdd15fffff2",Name="A207",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c4fghdf8da4849sdg35gggdd16fffff2",Name="A208",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                // Toà B Luxury
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff001",Name="B101",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff002",Name="B102",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff003",Name="B103",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff004",Name="B104",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bbdfggggfdbbefffsggdfffff005",Name="B105",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff006",Name="B106",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff007",Name="B107",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff008",Name="B108",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff100",Name="B201",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff110",Name="B202",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff120",Name="B203",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff130",Name="B204",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff140",Name="B205",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff150",Name="B206",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff160",Name="B207",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff170",Name="B208",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                // Toà C Premium
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff200",Name="C101",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff210",Name="C102",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff220",Name="C103",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff230",Name="C104",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff240",Name="C105",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710",HouseTypeID="c401bb08da4849sd346tgbdfgc271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff250",Name="C106",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff260",Name="C107",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff270",Name="C108",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff280",Name="C201",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff290",Name="C202",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff300",Name="C203",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711",HouseTypeID="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff310",Name="C204",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff320",Name="C205",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff330",Name="C206",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff340",Name="C207",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff350",Name="C208",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712",HouseTypeID="c401bb08da4849sdg35ggggdfffffff2"},

            };

            foreach (var item in rooms)
            {

                if (!await _unitOfWork.GetRepository<Room>().Entities.AnyAsync(i => i.Id == item.Id))
                {
                    item.Status = EnumRoom.Uninhabited;
                    item.IsActive = true;
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<Room>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task addFacilities()
        {
            Facilities[] facilities =
            {
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff000",Name="Khen tắm",Description="Khen tắm",Price=150000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff001",Name="Giường ngủ",Description="",Price=10000000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff002",Name="Tủ lạnh panasonic",Description="",Price=7000000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff003",Name="Máy lạnh panasonic",Description="",Price=6500000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff004",Name="Máy giặt panasonic",Description="",Price=6900000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff005",Name="Máy sấy tóc",Description="",Price=400000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff006",Name="Bàn ủi",Description="",Price=460000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff007",Name="Ấm đun nước",Description="",Price=250000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff008",Name="TV màn hình phẳng",Description="",Price=11500000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff009",Name="Bàn làm việc",Description="",Price=1500000},
                new Facilities(){Id="c401bb08da4849sdg35ggggdfffff010",Name="Đồ vệ sinh cá nhân",Description="",Price=200000},
            };
            foreach (var item in facilities)
            {

                if (!await _unitOfWork.GetRepository<Facilities>().Entities.AnyAsync(i => i.Id == item.Id))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<Facilities>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        
        private async Task addFacilitiesRoom()
        {
            List<Room> rooms=await _unitOfWork.GetRepository<Room>().Entities.ToListAsync();
            List<Facilities> facilities=await _unitOfWork.GetRepository<Facilities>().Entities.ToListAsync();
            if (rooms != null && facilities != null)
            {
                foreach (var room in rooms)
                {
                    foreach (var facility in facilities)
                    {
                        FacilitiesRoom facilityRoom = new FacilitiesRoom()
                        {
                            RoomID = room.Id,
                            FacilitiesID=facility.Id,
                            Quantity=1
                        };
                        await _unitOfWork.GetRepository<FacilitiesRoom>().InsertAsync(facilityRoom);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
