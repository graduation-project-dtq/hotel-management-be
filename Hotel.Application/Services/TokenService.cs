using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hotel.Application.DTOs.UserDTO;
using Hotel.Application.Interfaces;
using Hotel.Core.Constants;
using Hotel.Core.Exceptions;
using Hotel.Domain.Entities;
using Hotel.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace Hotel.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        public TokenResponseDto GenerateToken(Account account, string role)
        {
            DateTime now = DateTime.Now;
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim("role",role),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(now.AddMinutes(60)).ToUnixTimeSeconds().ToString())
            };
            var keyString = _configuration["JWT_KEY"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var claimsIdentity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(new[] { claimsIdentity });
            _httpContextAccessor.HttpContext.User = principal;

            Console.WriteLine("CHECK KEY: " + key);
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            // Access token
            var accessToken = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                expires: DateTime.Now.AddMinutes(int.Parse(_configuration["JWT_ACCESS_TOKEN_EXPIRATION_MINUTES"])),
                signingCredentials: creds
            );

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);

            // Refresh token
            var refreshToken = new JwtSecurityToken(
                claims: claims,
                issuer: _configuration["JWT_ISSUER"],
                audience: _configuration["JWT_AUDIENCE"],
                expires: DateTime.Now.AddDays(int.Parse(_configuration["JWT_REFRESH_TOKEN_EXPIRATION_DAYS"])),
                signingCredentials: creds
            );

            var refreshTokenString = new JwtSecurityTokenHandler().WriteToken(refreshToken);

            //RoleName
            var roleId = _unitOfWork.GetRepository<Role>().GetByIdAsync(account.RoleId ?? null).Result;
            var roleName = role != null ? roleId.RoleName : string.Empty;

            return new TokenResponseDto
            {
                AccessToken = accessTokenString,
                RefreshToken = refreshTokenString,

                Account = new ResponseAccountDto
                {
                    Id = account.Id,
                    Email = account.Email,
                    Role = roleName,
                    CreatedTime = account.CreatedTime,
                }
            };
        }
        public async Task<TokenResponseDto> RefreshAccessToken(RefeshTokenRequestDto refeshTokenRequest)
        {
            var principal = GetPrincipalFromExpiredToken(refeshTokenRequest.RefreshToken);
            if (principal == null)
            {
                throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.INVALID_TOKEN, "Invalid refresh token");
            }
            var accountId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var account = await _unitOfWork.GetRepository<Account>().Entities
                .FirstOrDefaultAsync(x => x.Id == accountId && x.DeletedTime == null);

            if (account == null)
            {
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Account not found");
            }

            var role = await _unitOfWork.GetRepository<Role>().Entities
                .FirstOrDefaultAsync(x => x.Id == account.RoleId && x.DeletedTime == null) ??
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, "Role not found for the account");

            var newTokens = GenerateToken(account, role.RoleName);

            return newTokens;
        }

        // Xác thực refresh token
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT_KEY"])),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ErrorException(StatusCodes.Status401Unauthorized, ResponseCodeConstants.INVALID_TOKEN, "Invalid token");
                }

                return principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.GetType().Name} - {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return null;
            }
        }
    }
}