using Hotel.Contract.Repositories.Entity;
using Hotel.Core.App;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Repositories.Context
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(DatabaseContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Tạo các vai trò nếu chưa tồn tại
            var roles = new[] { AppRole.Administrator, AppRole.Customer, AppRole.DefaultRole };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                    {
                        Console.WriteLine($"Error creating role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Tạo một người dùng mặc định nếu chưa tồn tại
            var defaultUserEmail = "admin@admin.com";
            var defaultUser = await userManager.FindByEmailAsync(defaultUserEmail);
            if (defaultUser == null)
            {
                defaultUser = new User
                {
                    UserName = defaultUserEmail,
                    Email = defaultUserEmail,
                    FullName = "Admin User",
                    IsActive = true
                };

                var userResult = await userManager.CreateAsync(defaultUser, "Admin123@");
                if (userResult.Succeeded)
                {
                    // Chỉ thêm vai trò cho người dùng khi vai trò đã được tạo thành công
                    foreach (var role in roles)
                    {
                        if (await roleManager.RoleExistsAsync(role) && !await userManager.IsInRoleAsync(defaultUser, role))
                        {
                            var roleAddResult = await userManager.AddToRoleAsync(defaultUser, role);
                            if (!roleAddResult.Succeeded)
                            {
                                Console.WriteLine($"Error adding role {role} to user {defaultUserEmail}: {string.Join(", ", roleAddResult.Errors.Select(e => e.Description))}");
                            }
                        }
                    }
                }
                else
                {
                    // Xử lý lỗi nếu người dùng không được tạo thành công
                    Console.WriteLine($"Error creating user {defaultUserEmail}: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
