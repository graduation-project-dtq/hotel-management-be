using Castle.Core.Logging;
using Hotel.Contract.Repositories.Entity;
using Hotel.Contract.Services.IService;
using Hotel.Core.App;
using Hotel.ModelViews.AccountModelView;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountService> _logger;
        public AccountService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration,ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<IdentityResult> SignUp(SignUpModelView model)
        {
            try
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    IsActive = true
                };

                // Tạo người dùng
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Kiểm tra và tạo vai trò nếu chưa tồn tại
                    if (!await _roleManager.RoleExistsAsync(AppRole.Customer))
                    {
                        var roleResult = await _roleManager.CreateAsync(new IdentityRole(AppRole.Customer));
                        if (!roleResult.Succeeded)
                        {
                            // Log lỗi khi tạo vai trò không thành công
                            _logger.LogError("Failed to create role: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                            return IdentityResult.Failed(new IdentityError { Description = "An error occurred while creating the role." });
                        }
                    }

                    // Thêm vai trò cho người dùng
                    var addRoleResult = await _userManager.AddToRoleAsync(user, AppRole.Customer);
                    if (!addRoleResult.Succeeded)
                    {
                        // Log lỗi khi thêm vai trò không thành công
                        _logger.LogError("Failed to add role to user: {Errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                        return IdentityResult.Failed(new IdentityError { Description = "An error occurred while assigning the role." });
                    }
                }
                else
                {
                    // Log lỗi khi tạo người dùng không thành công
                    _logger.LogError("Failed to create user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log lỗi chung
                _logger.LogError(ex, "An error occurred while signing up a new user.");
                return IdentityResult.Failed(new IdentityError { Description = "An error occurred while processing your request." });
            }
        }


        public async Task<string> SignIn(SignInModelView model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                // Lấy danh sách các vai trò của người dùng
                var roles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                // Thêm các vai trò vào claims
                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(authClaims),
                    Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:TokenExpirationInMinutes"])),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["JWT:ValidIssuer"],
                    Audience = _configuration["JWT:ValidAudience"]
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }

            return null;
        }
    }
}
