using Microsoft.Extensions.Configuration;
using Hotel.Contract.Repositories.IUOW;
using Hotel.Contract.Services.IService;
using Hotel.Core.App;
using Hotel.ModelViews.AccountModelView;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Hotel.Contract.Repositories.Entity;
using System;
using System.Collections.Generic;
using Hotel.Core.Utils;

namespace Hotel.Services.Service
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountService(IUnitOfWork unitOfWork, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        // Đăng nhập và trả về token JWT
        public async Task<string> SignInAsync(SignInViewModel signInViewModel)
        {
            var user = await _userManager.FindByEmailAsync(signInViewModel.UserName);
            if (user == null)
            {
                return "User not found";
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, signInViewModel.Password);
            if (!passwordValid)
            {
                return "Password not validated"; // Hoặc thông báo lỗi phù hợp
            }
            // Tạo danh sách các claims (quyền hạn)
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

            // Kiểm tra xem người dùng đã có token hợp lệ chưa
            var existingToken = await GetTokenAsync(user);

            // Kiểm tra thời gian hết hạn của token hiện tại (Nếu token không có thông tin về thời gian hết hạn, bạn cần thêm vào cơ sở dữ liệu)
            if (!string.IsNullOrEmpty(existingToken))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token1 = tokenHandler.ReadJwtToken(existingToken);
                var exp = token1.ValidTo;

                // Nếu token còn hiệu lực, trả về token hiện tại
                if (exp > DateTime.UtcNow)
                {
                    return existingToken;
                }
            }
            // Tạo token mới
            var authenticationKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(7), // Thay đổi thời gian hết hạn theo nhu cầu
                claims: new List<Claim>
                {
            new Claim(ClaimTypes.Email, signInViewModel.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                }.Union(await GetUserClaims(user)),
                signingCredentials: new SigningCredentials(authenticationKey, SecurityAlgorithms.HmacSha512Signature)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Lưu token mới vào cơ sở dữ liệu
            await SaveTokenAsync(user, tokenString);

            return tokenString;
        }



        // Lưu token vào cơ sở dữ liệu
        public async Task SaveTokenAsync(User user, string token)
        {
            // Xóa token cũ (nếu có) trước khi lưu token mới
            await _userManager.RemoveAuthenticationTokenAsync(user, "JWT", "Token");

            // Lưu token mới
            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "Token", token);
        }

        // Lấy token từ cơ sở dữ liệu
        public async Task<string> GetTokenAsync(User user)
        {
            var token = await _userManager.GetAuthenticationTokenAsync(user, "JWT", "Token");
            return token;
        }
        // Tạo các claims cho người dùng
        private async Task<IEnumerable<Claim>> GetUserClaims(User user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>();

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        // Đăng ký người dùng mới
        public async Task<IdentityResult> SignUpAsync(SignUpViewModel signUpViewModel)
        {
            if (signUpViewModel == null)
            {
                throw new ArgumentNullException(nameof(signUpViewModel));
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString("N"),
                Fullname = signUpViewModel.FullName,
                Email = signUpViewModel.Email,
                UserName = signUpViewModel.Email
            };

            var result = await _userManager.CreateAsync(user, signUpViewModel.Password);

            if (result.Succeeded)
            {
                // Kiểm tra và tạo các Role nếu chưa tồn tại
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

                // Gán vai trò mặc định cho người dùng
                await _userManager.AddToRoleAsync(user, AppRole.DefaultRole);
            }
            return result;
        }
        //#region TEST
        //public async Task<string> SignInAsync(SignInViewModel signInViewModel)
        //{
        //    var user = await _userManager.FindByEmailAsync(signInViewModel.UserName);
        //    var passwordValid = await _userManager.CheckPasswordAsync(user, signInViewModel.Password);

        //    if (!passwordValid || user is null)
        //    {
        //        return string.Empty;
        //    }

        //    var authClaims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Email, signInViewModel.UserName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        //    };

        //    var userRoles = await _userManager.GetRolesAsync(user);
        //    foreach (var role in userRoles)
        //    {
        //        authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        //    }

        //    var authenticationKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["JWT:ValidIssuer"],
        //        audience: _configuration["JWT:ValidAudience"],
        //        expires: DateTime.Now.AddHours(7),
        //        claims: authClaims,
        //        signingCredentials: new SigningCredentials(authenticationKey, SecurityAlgorithms.HmacSha512Signature)
        //        );
        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        //public async Task<IdentityResult> SignUpAsync(SignUpViewModel signUpViewModel)
        //{
        //    var user = new User
        //    {
        //        Id = Guid.NewGuid().ToString("N"),
        //        Fullname = signUpViewModel.FullName,
        //        Email = signUpViewModel.Email,
        //        UserName = signUpViewModel.Email
        //    };

        //    var result = await _userManager.CreateAsync(user, signUpViewModel.Password);

        //    if (result.Succeeded)
        //    {
        //        // Kiểm tra các Role đã tồn tại
        //        if (!await _roleManager.RoleExistsAsync(AppRole.Administrator))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(AppRole.Administrator));
        //        }
        //        if (!await _roleManager.RoleExistsAsync(AppRole.Customer))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(AppRole.Customer));
        //        }
        //        if (!await _roleManager.RoleExistsAsync(AppRole.DefaultRole))
        //        {
        //            await _roleManager.CreateAsync(new IdentityRole(AppRole.DefaultRole));
        //        }
        //        await _userManager.AddToRoleAsync(user, AppRole.DefaultRole);
        //    }
        //    return result;
        //}
        //#endregion
    }
}
