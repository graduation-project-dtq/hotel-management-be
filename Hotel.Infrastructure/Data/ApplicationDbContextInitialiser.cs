using Hotel.Core.Base;
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
                await addRoomTypeDetail();
                await addFloor();
                await addRoom();
                await addFacilities();
                await addFacilitiesRoom();

                await addImageRoomType();
                await addImageRoomTypeDetail();
                //Service
                await addService();
                await addImageService();
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
                
                new Image{Id="51be69a4b2144a8987551523d428bd01",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/552793120.jpg?k=663189fe3f75a9af8d9f525b226982c51220a7807ba8c7a63de6479569dfc3db&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987551523d428bd02",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/578299549.jpg?k=5420f7cfb664a5fd90c1b0d74a5c1ae3bdcd98e86e7f595d0cb8cd56bcc332e3&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987551523d428bd03",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/528500991.jpg?k=72b209ae2048ecdcaa617e9dbc675c0a404c8cb2d8207fdf3a49b18804ab2800&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987551523d428bd04",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/528504264.jpg?k=69e03ece617b733ecdd563f2c350c0ea4e1fbc63e14499920d3805f407f0ea3e&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987551523d428bd05",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/552793017.jpg?k=fc143eabd12fe5c54622159e55b5ff568bb51fa7352f10434495e45bfdc4bba3&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987551523d428bd06",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/552793167.jpg?k=2aed1bb80a4e81ae44c15f1212aa9d7e414bb52dba79149777b952bf8f27df32&o=&hp=1"},
                new Image{Id="51be69a4b2144a8987551523d428bd07",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/552791441.jpg?k=60f86132046c3d4f5599b579b0f9206cdf88110384328170ec9fe2dbfdaf7415&o=&hp=1"},
                
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
                new Image{Id="51be69a4bdgf4a8987551569a428sddfg",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/479059384.jpg?k=bf60975ceea7dd208d2133889fbd6bfa708e4161f82998278866db2fb8b7c950&o=&hp=1"},
                //-------------------
                //Standard 1 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b001",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/590831949.jpg?k=5e8df42e84d4cc77633b5eb66389c26cb0156e92161bdcb89f0439676239f901&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b002",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/590834402.jpg?k=a2217fa4e73e9e11b1199c9fd415f3d81c9007446d40d4b38bf538ca456d7bef&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b003",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/590832730.jpg?k=601ca91cdd26d3fcb6bf27c75203ae89395473b60e5317545b2c044f3f99c68a&o=&hp=1"},
                //Standard 2 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b004",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/593517869.jpg?k=f36f215cf41a15b2a1a732ae5f87d5da1012da0ed02626855f8fd9524f80ae3d&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b005",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/593517861.jpg?k=01286929959a1eb024a9f6a5c5197d92c3e9bdbbc4507d330a1166117b114660&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b006",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/601817180.jpg?k=585140c2a8cd89bb4cf5c067c35facc006509f57991a28495ea82cfdd3cd2db2&o=&hp=1"},
                //Standard 3 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b007",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/548661377.jpg?k=6e739537c0ee6150c12a0cae845facdb2a20db00a4348dcaaea017efb5e98863&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b008",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/548652007.jpg?k=8fb022a1496834097e03dddf4af7eb0aaa34a8cee3f3d348a11174384e2f5959&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b009",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/548653430.jpg?k=9a5400ea60b96901a701f042f4e4eb51c8080c7f7a088c32a6ad5e141d185021&o=&hp=1"},
                //Luxyry 1 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b010",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/452645427.jpg?k=673b0239487dd10e41c7311ce1cf2ce7a568819db4d5c82504fede98da64ed5e&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b011",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/452674600.jpg?k=04a9660efa504bfb621119fd88bc5daa51aeda450c26b3b3af8f9e1d6b1e1f2e&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b012",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/406896397.jpg?k=0b27bbb7c2e9669711bd2a3f8fc8c6a7930b3aa0cdb5cbf4f127f79c09354030&o=&hp=1"},
                 //Luxyry 2 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b013",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/533340308.jpg?k=b11407bca9b0021a8b44277f52ac7e92d2579afbdee3e32c5d33d6ca2dbcbf7a&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b014",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/599490153.jpg?k=36d9563a4dee3c70845c8ba356424aa1acc130ce61baba6060a6fb79b361ba80&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b015",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/599820360.jpg?k=9d6c98d74c36755e2a470141ab586ae9a05ad4ca1a621d44e45e8c9ed6d17519&o=&hp=1"},
 
                //Luxyry 3 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b016",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/531973996.jpg?k=8aad3fd8f825806bc8f843d783b64074b0d326bb4f7bd1dcf069a2da44950fe5&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b017",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/531974011.jpg?k=958b19838cde9d0e86a256f416edfc1b968f40630ce652aa40f73fbd9e4f69f3&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b018",URL="https://cf.bstatic.com/xdata/images/hotel/max1280x900/597969691.jpg?k=657ef776dfc7f67b743b5d2dd870022839c8dfd0e0852e7049411b97680d0397&o=&hp=1"},

                //Premium 1 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b019",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/598533733.jpg?k=a642e3b55cb705c6b180e8173d26f1849a8585fa2b5610767ab51a4bce63f16e&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b020",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/598275920.jpg?k=c0c0008dd9d1d90d60e6440d33f0ef3944674b10ffa4a551c9433c2e91af0978&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b021",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/598275930.jpg?k=9be4b130d4fc98cc356641c10da9f197ecdfa3b32e91a7bc9fb9a991d4496b4f&o=&hp=1"},

                //Premium 2 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b022",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/553666165.jpg?k=e8b3f5407d340fa62886f22f71d15a03f2663421e8273517cc12aba11570b67c&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b023",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/598275920.jpg?k=c0c0008dd9d1d90d60e6440d33f0ef3944674b10ffa4a551c9433c2e91af0978&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b024",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/438942876.jpg?k=64dad9668a83ae01aae27d8884339f800f82a082d123aec88e84bfb8efdf38aa&o=&hp=1"},

                //Premium 3 phòng
                new Image{Id="51be69a4bdgf4a8987551569a428b025",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/598275920.jpg?k=c0c0008dd9d1d90d60e6440d33f0ef3944674b10ffa4a551c9433c2e91af0978&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b026",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/532299206.jpg?k=0ae077ef9569b9a183d46bac3737840b456d976a1716e153d1389adba00d62ee&o=&hp=1"},
                new Image{Id="51be69a4bdgf4a8987551569a428b027",URL="https://cf.bstatic.com/xdata/images/hotel/max1024x768/572015603.webp?k=68a1d98b469233ac7a09af3647a190aeb8cdb15f35f0dc5ff35310e0ab1debbd&o="},
            
                //Dịch vụ

                //Ăn sáng
                new Image{Id="51be69a4bdgf4a89875515service001",URL="https://lotushotel.vn/wp-content/uploads/2021/01/thuc-don-buffet-sang-cho-du-khach-tai-khach-san-day-du-dinh-duong.jpg"},
                new Image{Id="51be69a4bdgf4a89875515service002",URL="https://thietbidungcubuffet.com/images/tin-tuc/thuc-khach-thoai-mai-lua-chon-mon-an.jpg"},
                //Thuê xe máy
                new Image{Id="51be69a4bdgf4a89875515service003",URL="https://the-onehotel.vn/wp-content/uploads/2016/12/cho-thue-xe-motor.png"},
                new Image{Id="51be69a4bdgf4a89875515service004",URL="https://the-onehotel.vn/wp-content/uploads/2020/07/dua-don-san-bay-nha-trang-1-2.jpg"},
                //Xe điện đưa đón
                new Image{Id="51be69a4bdgf4a89875515service005",URL="https://tunglamco.com.vn/wp-content/uploads/2020/08/xe-dien-cho-khach-du-lich-2.jpg"},
                new Image{Id="51be69a4bdgf4a89875515service006",URL="https://tunglamco.com.vn/wp-content/uploads/2022/05/Xe-dien-resort-Phuong-tien-cho-khach-toi-uu-cho-khu-nghi-duong.png"},
                //Massage thư giãn
                new Image{Id="51be69a4bdgf4a89875515service007",URL="https://songdaiduong.com/wp-content/uploads/2020/03/spa-s%E1%BA%A7m-s%C6%A1n.jpg"},
                new Image{Id="51be69a4bdgf4a89875515service008",URL="https://hssc.vn/wp-content/uploads/2023/04/3-1.png"},
                //Dịch vụ giặt ủi
                new Image{Id="51be69a4bdgf4a89875515service009",URL="https://lotushotel.vn/wp-content/uploads/2021/07/quy-trinh-su-dung-dich-vu-giat-la-tai-khach-san.jpg"},
                new Image{Id="51be69a4bdgf4a89875515service010",URL="https://lotushotel.vn/wp-content/uploads/2021/01/vai-tro-cua-bo-phan-giat-la-tai-khach-san-so-huu-nhan-vien-co-suc-khoe-tot.jpg"},
                //Gym và Fitness
                new Image{Id="51be69a4bdgf4a89875515service011",URL="https://www.lottehotel.com/content/dam/lotte-hotel/lotte/saigon/facilities/fitness-spa/hotelgym/6647-2-2000-fac-LTHO.jpg.thumb.1920.1920.jpg"},
                new Image{Id="51be69a4bdgf4a89875515service012",URL="https://www.lottehotel.com/content/dam/lotte-hotel/lotte/saigon/facilities/fitness-spa/hotelgym/6647-4-2000-fac-LTHO.jpg.thumb.1920.1920.jpg"},
                //Dịch vụ hồ bơi
                new Image{Id="51be69a4bdgf4a89875515service013",URL="https://www.seagullhotel.com.vn/wp-content/uploads/2021/10/Ho-boi-Khach-san-Hai-Au-Quy-Nhon-Seagull-Hotel-1.jpg"},
                new Image{Id="51be69a4bdgf4a89875515service014",URL="https://hoboisaigon.com/hoanghung/5/images/901.jpg"},

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
                
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bd01"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bd02"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bd03"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bd04"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bd05"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bd06"},
                new ImageRoomType{RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",ImageID="51be69a4b2144a8987551523d428bd07"},
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
                new ImageRoomType{RoomTypeID="c401bb08da484925900a63575c2717f8",ImageID="51be69a4bdgf4a8987551569a428b002"},
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
       
        public async Task addRoomTypeDetail()
        {
            RoomTypeDetail[] roomTypeDetails =
            {
                //Standard
                new RoomTypeDetail(){Id="c401bb08da484fdgdggfsv575c2717f8",BasePrice=500000,Name="Standard 1 giường ngủ",RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",CapacityMax=2,Area=50,AverageStart=0,Description="Một giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},
                new RoomTypeDetail(){Id="c401bb08dfggggggrfffsv575c2717f8",BasePrice=700000,Name="Standard 2 giường ngủ",RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",CapacityMax=4,Area=64,AverageStart=0,Description="Hai giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},
                new RoomTypeDetail(){Id="c401bb08da48493erfffsv575c2717f9",BasePrice=1000000,Name="Standard 3 giường ngủ",RoomTypeID="11c1b04e29524abdbebd96ec80d6bc58",CapacityMax=6,Area=70,AverageStart=0,Description="Ba giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},

                //Luxury
                new RoomTypeDetail(){Id="c401bb08dasgsdgdsggdgggg5c2717f8",BasePrice=600000,Name="Luxury 1 giường ngủ",RoomTypeID="51be69a4b2144a8987551569a428b064",CapacityMax=2,Area=55,AverageStart=0,Description="Một giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},
                new RoomTypeDetail(){Id="c401bb08dasdgswfsvwawfffhs271710",BasePrice=800000,Name="Luxury 2 giường ngủ",RoomTypeID="51be69a4b2144a8987551569a428b064",CapacityMax=4,Area=66,AverageStart=0,Description="Hai giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},
                new RoomTypeDetail(){Id="c401bb08dfdgggggggffsv575c2717f8",BasePrice=1100000,Name="Luxury 3 giường ngủ",RoomTypeID="51be69a4b2144a8987551569a428b064",CapacityMax=6,Area=74,AverageStart=0,Description="Ba giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},

                //Premium
                new RoomTypeDetail(){Id="c401bb08da4sddggggggfv575c271710",BasePrice=700000,Name="Premium 1 giường ngủ",RoomTypeID="c401bb08da484925900a63575c2717f8",CapacityMax=2,Area=60,AverageStart=0,Description="Một giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},
                new RoomTypeDetail(){Id="c401bb08da48493erdffsv575c271711",BasePrice=900000,Name="Premium 2 giường ngủ",RoomTypeID="c401bb08da484925900a63575c2717f8",CapacityMax=4,Area=70,AverageStart=0,Description="Hai giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},
                new RoomTypeDetail(){Id="c401bb08da4849sdg35gggg75c271712",BasePrice=1400000,Name="Premium 3 giường ngủ",RoomTypeID="c401bb08da484925900a63575c2717f8",CapacityMax=6,Area=80,AverageStart=0,Description="Ba giường đôi lớn, Bếp riêng,Phòng tắm riêng, Ban công, Nhìn ra ban công, Điều hòa không khí, TV màn hình phẳng, Máy giặc, Hệ thống cách âm, Tiện nghi BBQ, WiFi miễn phí"},
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
    
        private async Task addImageRoomTypeDetail()
        {
            ImageRoomTypeDetail[] imageRoomTypeDetails =
            {
                //Standard 1 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b001",RoomTypeDetailID="c401bb08da484fdgdggfsv575c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b002",RoomTypeDetailID="c401bb08da484fdgdggfsv575c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b003",RoomTypeDetailID="c401bb08da484fdgdggfsv575c2717f8"},
                //Standard 2 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b004",RoomTypeDetailID="c401bb08dfggggggrfffsv575c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b005",RoomTypeDetailID="c401bb08dfggggggrfffsv575c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b006",RoomTypeDetailID="c401bb08dfggggggrfffsv575c2717f8"},
                //Standard 3 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b007",RoomTypeDetailID="c401bb08da48493erfffsv575c2717f9"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b008",RoomTypeDetailID="c401bb08da48493erfffsv575c2717f9"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b009",RoomTypeDetailID="c401bb08da48493erfffsv575c2717f9"},
                //Luxury 1 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b010",RoomTypeDetailID="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b011",RoomTypeDetailID="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b012",RoomTypeDetailID="c401bb08dasgsdgdsggdgggg5c2717f8"},
                //Luxury 2 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b013",RoomTypeDetailID="c401bb08dasdgswfsvwawfffhs271710"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b014",RoomTypeDetailID="c401bb08dasdgswfsvwawfffhs271710"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b015",RoomTypeDetailID="c401bb08dasdgswfsvwawfffhs271710"},
                //Luxury 3 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b016",RoomTypeDetailID="c401bb08dfdgggggggffsv575c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b017",RoomTypeDetailID="c401bb08dfdgggggggffsv575c2717f8"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b018",RoomTypeDetailID="c401bb08dfdgggggggffsv575c2717f8"},
                //Premium 1 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b019",RoomTypeDetailID="c401bb08da4sddggggggfv575c271710"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b020",RoomTypeDetailID="c401bb08da4sddggggggfv575c271710"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b021",RoomTypeDetailID="c401bb08da4sddggggggfv575c271710"},
                //Premium 2 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b022",RoomTypeDetailID="c401bb08da48493erdffsv575c271711"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b023",RoomTypeDetailID="c401bb08da48493erdffsv575c271711"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b024",RoomTypeDetailID="c401bb08da48493erdffsv575c271711"},
                //Premium 3 phòng
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b025",RoomTypeDetailID="c401bb08da4849sdg35gggg75c271712"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b026",RoomTypeDetailID="c401bb08da4849sdg35gggg75c271712"},
                new  ImageRoomTypeDetail(){ImageID="51be69a4bdgf4a8987551569a428b027",RoomTypeDetailID="c401bb08da4849sdg35gggg75c271712"},
            };
            foreach (var item in imageRoomTypeDetails)
            {

                if (!await _unitOfWork.GetRepository<ImageRoomTypeDetail>().Entities.AnyAsync(i => i.ImageID == item.ImageID && i.RoomTypeDetailID==item.RoomTypeDetailID))
                {
                
                    await _unitOfWork.GetRepository<ImageRoomTypeDetail>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
        private async Task addFloor()
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
        
        public async Task addRoom()
        {
            Room[] rooms =
            {
                // Toà A Standard
                new Room(){Id="c401bbdffa4849sdg35gggdd1ffffff2",Name="A101",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8"},
                new Room(){Id="c401bb08da4849sdg35gggdd2ffffff2",Name="A102",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8"},
                new Room(){Id="ffffbb08da4849sdg35gggdd3ffffff2",Name="A103",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8"},
               // new Room(){Id="c4fghdf8da4849sdg35gggdd4ffffff2",Name="A104",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8"},
               // new Room(){Id="c401bbdfggggfdbbefffsgg5ff4ffff2",Name="A105",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da484fdgdggfsv575c2717f8"},
                new Room(){Id="c401bb08da4849sdg35gggdd6ffffff2",Name="A106",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8"},
                new Room(){Id="ffffbb08da4849sdg35gggdd7ffffff2",Name="A107",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8"},
                new Room(){Id="c4fghdf8da4849sdg35gggdd8ffffff2",Name="A108",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8"},
                new Room(){Id="c401bbdffa4849sdg35gggdd9ffffff2",Name="A201",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8"},
                new Room(){Id="c401bb08da4849sdg35gggdd10fffff2",Name="A202",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8"}, //
                new Room(){Id="ffffbbfggg4849sdg35gggdd11fffff2",Name="A203",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08dfggggggrfffsv575c2717f8"},
                new Room(){Id="c4fghdf8da4849sdg35gggdd12fffff2",Name="A204",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9"},
                new Room(){Id="c401bbdffa4849sdg35gggdd13fffff2",Name="A205",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9" },
                new Room(){Id="c401bb08da4849sdg35gggdd14fffff2",Name="A206",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9"},
                new Room(){Id="ffffbb08da4849sdg35gggdd15fffff2",Name="A207",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9",},
                new Room(){Id="c4fghdf8da4849sdg35gggdd16fffff2",Name="A208",FloorId="c401bb08da4849sdg35ggggdfffffff2",RoomTypeDetailId="c401bb08da48493erfffsv575c2717f9"},
                // Toà B Luxury
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff001",Name="B101",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff002",Name="B102",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff003",Name="B103",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff004",Name="B104",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new Room(){Id="c401bbdfggggfdbbefffsggdfffff005",Name="B105",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff006",Name="B106",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasgsdgdsggdgggg5c2717f8"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff007",Name="B107",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff008",Name="B108",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710"},
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff100",Name="B201",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff110",Name="B202",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff120",Name="B203",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dasdgswfsvwawfffhs271710"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff130",Name="B204",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8"},
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff140",Name="B205",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff150",Name="B206",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8"},
                new Room(){Id="ffffbb08da4849sdg35ggggdfffff160",Name="B207",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8"},
                new Room(){Id="c4fghdf8da4849sdg35ggggdfffff170",Name="B208",FloorId="c401bb08da4849sd346tgbdfgc271712",RoomTypeDetailId="c401bb08dfdgggggggffsv575c2717f8"},
                // Toà C Premium
                new Room(){Id="c401bbdffa4849sdg35ggggdfffff200",Name="C101",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff210",Name="C102",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff220",Name="C103",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff230",Name="C104",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff240",Name="C105",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4sddggggggfv575c271710"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff250",Name="C106",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff260",Name="C107",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff270",Name="C108",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff280",Name="C201",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff290",Name="C202",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff300",Name="C203",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da48493erdffsv575c271711"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff310",Name="C204",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff320",Name="C205",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff330",Name="C206",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff340",Name="C207",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712"},
                new Room(){Id="c401bb08da4849sdg35ggggdfffff350",Name="C208",FloorId="c401bb08da4849sdg35gggg75c271712",RoomTypeDetailId="c401bb08da4849sdg35gggg75c271712"},

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
            List<Room> rooms = await _unitOfWork.GetRepository<Room>().Entities.ToListAsync();
            List<Facilities> facilities = await _unitOfWork.GetRepository<Facilities>().Entities.ToListAsync();

            if (rooms != null && facilities != null)
            {
                foreach (var room in rooms)
                {
                    foreach (var facility in facilities)
                    {
                        // Kiểm tra xem bản ghi đã tồn tại hay chưa
                        var existingFacilityRoom = await _unitOfWork.GetRepository<FacilitiesRoom>()
                            .Entities
                            .FirstOrDefaultAsync(fr => fr.RoomID == room.Id && fr.FacilitiesID == facility.Id);

                        if (existingFacilityRoom == null)
                        {
                            // Nếu không tồn tại, thêm mới bản ghi
                            FacilitiesRoom facilityRoom = new FacilitiesRoom()
                            {
                                RoomID = room.Id,
                                FacilitiesID = facility.Id,
                                Quantity = 1
                            };

                            await _unitOfWork.GetRepository<FacilitiesRoom>().InsertAsync(facilityRoom);
                        }
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                await _unitOfWork.SaveChangesAsync();
            }
        }
        private async Task addService()
        {
            Service[] services =
            {
                new Service(){Id="c401bb08da4849sdg35fhdgdfffff001", Name="Ăn sáng", Price=1000000, Description="Bữa ăn sáng chất lượng nhà hàng 3 sao"},
                new Service(){Id="c401bb08da4849sdg35fhdgdfffff002", Name="Thuê xe máy", Price=120000, Description="Xe máy là những dòng xe ga cao cấp, an toàn cho khách hàng"},
                new Service(){Id="c401bb08da4849sdg35fhdgdfffff003", Name="Xe điện đưa đón", Price=30000, Description="Xe điện sẽ chở khách hàng đi tham quan xung quanh khách sạn và những địa điểm lân cận"},
                new Service(){Id="c401bb08da4849sdg35fhdgdfffff004", Name="Massage thư giãn", Price=500000, Description="Dịch vụ massage thư giãn toàn thân, phục hồi sức khỏe sau chuyến đi"},
                new Service(){Id="c401bb08da4849sdg35fhdgdfffff005", Name="Dịch vụ giặt ủi", Price=200000, Description="Giặt ủi quần áo chuyên nghiệp, trả đồ trong ngày"},
                new Service(){Id="c401bb08da4849sdg35fhdgdfffff006", Name="Gym và Fitness", Price=300000, Description="Sử dụng phòng gym với trang thiết bị hiện đại, mở cửa 24/7"},
                new Service(){Id="c401bb08da4849sdg35fhdgdfffff007", Name="Dịch vụ hồ bơi", Price=0, Description="Miễn phí sử dụng hồ bơi trong khuôn viên khách sạn"}

            };
            foreach (var item in services)
            {

                if (!await _unitOfWork.GetRepository<Service>().Entities.AnyAsync(i => i.Id == item.Id))
                {
                    item.CreatedTime = CoreHelper.SystemTimeNow;
                    item.LastUpdatedTime = CoreHelper.SystemTimeNow;
                    await _unitOfWork.GetRepository<Service>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task addImageService()
        {
            ImageService[] imageServices =
            {
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff001",ImageID="51be69a4bdgf4a89875515service001"},
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff001",ImageID="51be69a4bdgf4a89875515service002"},

                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff002",ImageID="51be69a4bdgf4a89875515service003"},
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff002",ImageID="51be69a4bdgf4a89875515service004"},

                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff003",ImageID="51be69a4bdgf4a89875515service005"},
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff003",ImageID="51be69a4bdgf4a89875515service006"},

                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff004",ImageID="51be69a4bdgf4a89875515service007"},
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff004",ImageID="51be69a4bdgf4a89875515service008"},

                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff005",ImageID="51be69a4bdgf4a89875515service009"},
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff005",ImageID="51be69a4bdgf4a89875515service010"},

                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff006",ImageID="51be69a4bdgf4a89875515service011"},
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff006",ImageID="51be69a4bdgf4a89875515service012"},

                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff007",ImageID="51be69a4bdgf4a89875515service013"},
                new ImageService(){ServiceID="c401bb08da4849sdg35fhdgdfffff007",ImageID="51be69a4bdgf4a89875515service014"},

            };
            foreach (var item in imageServices)
            {

                if (!await _unitOfWork.GetRepository<ImageService>().Entities.AnyAsync(i => i.ServiceID == item.ServiceID && i.ImageID==item.ImageID))
                {
                   await _unitOfWork.GetRepository<ImageService>().InsertAsync(item);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
