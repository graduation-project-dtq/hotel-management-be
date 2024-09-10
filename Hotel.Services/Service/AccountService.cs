using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Hotel.Contract.Repositories.Entity;
using Hotel.Core.App;
using Hotel.ModelViews.AccountModelView;
using Hotel.Contract.Services.IService;

namespace Hotel.Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<string> SignInAsync(SignInViewModel signInViewModel)
        {
            var user = await _userManager.FindByNameAsync(signInViewModel.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, signInViewModel.Password))
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = CreateToken(authClaims);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            await SaveTokenAsync(user, tokenString);

            return tokenString;
        }


        public async Task<IdentityResult> SignUpAsync(SignUpViewModel signUpViewModel)
        {
            var user = new User
            {
                UserName = signUpViewModel.Email,
                Email = signUpViewModel.Email,
                Fullname = signUpViewModel.FullName
            };

            var result = await _userManager.CreateAsync(user, signUpViewModel.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, AppRole.DefaultRole);
            }

            return result;
        }

        public async Task SaveTokenAsync(User user, string token)
        {
            var result = await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", token);
            if (!result.Succeeded)
            {
                // Log or handle failure
                throw new Exception("Failed to save authentication token.");
            }
        }


        public async Task<string> GetTokenAsync(User user)
        {
            return await _userManager.GetAuthenticationTokenAsync(user, "JWT", "AccessToken");
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);


            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:ValidAudience"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task InitializeRolesAsync()
        {
            string[] roleNames = { AppRole.Administrator, AppRole.Customer, AppRole.DefaultRole };

            foreach (var roleName in roleNames)
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private JwtSecurityToken CreateToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(7),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha512Signature)
            );
        }
    }
}