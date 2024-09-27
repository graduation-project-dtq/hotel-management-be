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
    }
   
}
