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

        public async Task<string> SignInAsync(SignInModelView signInViewModel)
        {
            var user = await _userManager.FindByEmailAsync(signInViewModel.UserName);
            var passwordValid = await _userManager.CheckPasswordAsync(user, signInViewModel.Password);

            if (!passwordValid || user is null)
            {
                return string.Empty;
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, signInViewModel.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }

            var authenticationKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(7),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authenticationKey, SecurityAlgorithms.HmacSha512Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IdentityResult> SignUpAsync(SignUpModelView signUpViewModel)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString("N"),
                FullName = signUpViewModel.FullName,
                Email = signUpViewModel.Email,
                UserName = signUpViewModel.Email
            };

            var result = await _userManager.CreateAsync(user, signUpViewModel.Password);

            if (result.Succeeded)
            {
                // Kiểm tra các Role đã tồn tại
                if (!await _roleManager.RoleExistsAsync(AppRole.Administrator))
                {
                    await _roleManager.CreateAsync(new IdentityRole(AppRole.Administrator));
                }
                if (!await _roleManager.RoleExistsAsync(AppRole.Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(AppRole.Customer));
                }
                if (!await _roleManager.RoleExistsAsync(AppRole.DefaultRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole(AppRole.DefaultRole));
                }
                await _userManager.AddToRoleAsync(user, AppRole.DefaultRole);
            }
            return result;
        }
    }
}
