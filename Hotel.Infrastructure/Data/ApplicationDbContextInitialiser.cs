using Hotel.Core.Base;
using Hotel.Core.Common;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Hotel.Infrastructure.Data
{
    public class ApplicationDbContextInitialiser
    {
        private readonly HotelDBContext _context;
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        public ApplicationDbContextInitialiser(
               HotelDBContext context,
               ILogger<ApplicationDbContextInitialiser> logger,
               IUnitOfWork unitOfWork,
               UserManager<ApplicationUser> userManager,
               RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
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
            if (!await _roleManager.Roles.AnyAsync(r => r.DeletedTime == null))
            {
                ApplicationRole[] roles =
                {
                    new ApplicationRole { Id = Guid.Parse("a1234567-bcde-41fb-8635-1c19e1d08bd6"), Name = CLAIMS_VALUES.ROLE_TYPE.ADMIN },
                    new ApplicationRole { Id = Guid.Parse("b2345678-cdef-41fb-8635-1c19e1d08bd6"), Name = CLAIMS_VALUES.ROLE_TYPE.EMPLOYEE },
                    new ApplicationRole { Id = Guid.Parse("c3456789-def0-41fb-8635-1c19e1d08bd6"), Name = CLAIMS_VALUES.ROLE_TYPE.CUSTOMER },
                };

                foreach (ApplicationRole role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role.Name))
                    {
                        role.CreatedTime = CoreHelper.SystemTimeNow;
                        role.LastUpdatedTime = CoreHelper.SystemTimeNow;
                        var result = await _roleManager.CreateAsync(role);
                        if (!result.Succeeded)
                        {
                            // Xử lý lỗi nếu không thêm được vai trò
                            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                }
            }
        }


        private async Task addAccount()
        {
            FixedSaltPasswordHasher<ApplicationUser> passwordHasher = new FixedSaltPasswordHasher<ApplicationUser>(Options.Create(new PasswordHasherOptions()));

            if (!await _context.ApplicationUsers.AnyAsync(x => x.DeletedTime == null))
            {
                var organizerRole = await _roleManager.FindByNameAsync(CLAIMS_VALUES.ROLE_TYPE.ADMIN);
                if (organizerRole != null)
                {
                    ApplicationUser account = new ApplicationUser
                    {
                        Id = Guid.Parse("621acdb9-80da-41fb-8635-1c19e1d08bd6"),
                        Email = "admin@gmail.com",
                        UserName = "admin", // Thêm tên người dùng hợp lệ
                        Password = passwordHasher.HashPassword(null, "Admin@123"),
                        IsActive = true,
                        CreatedTime = CoreHelper.SystemTimeNow,
                        LastUpdatedTime = CoreHelper.SystemTimeNow
                    };

                    var result = await _userManager.CreateAsync(account, "Admin@123");
                    if (result.Succeeded)
                    {
                        // Thêm người dùng vào vai trò admin
                        await _userManager.AddToRoleAsync(account, CLAIMS_VALUES.ROLE_TYPE.ADMIN);
                    }
                    else
                    {
                        // Xử lý lỗi nếu không thêm được người dùng
                        throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }


        private async Task addUserRole()
        {
            ApplicationUserRoles applicationUserRoles = new ApplicationUserRoles
            {
                UserId = Guid.Parse("621acdb9-80da-41fb-8635-1c19e1d08bd6"),
                RoleId = Guid.Parse("a1234567-bcde-41fb-8635-1c19e1d08bd6"),
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
            await _unitOfWork.GetRepository<ApplicationUserRoles>().InsertAsync(applicationUserRoles);
            await _unitOfWork.SaveChangesAsync();
        }
    }
   
}
