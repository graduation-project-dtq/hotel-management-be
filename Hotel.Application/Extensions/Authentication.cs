using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Hotel.Application.Extensions
{
    public class Authentication
    {
        public static string GetUserIdFromHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor.HttpContext == null)
            {
                throw new UnauthorizedException("HTTP context is null. Authorization is required.");
            }

            // Kiểm tra header Authorization
            if (!httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                throw new UnauthorizedException("Authorization header is missing.");
            }

            // Chuyển đổi authorizationHeader sang string
            string authHeaderString = authorizationHeader.ToString();

            // Kiểm tra định dạng Bearer
            if (string.IsNullOrWhiteSpace(authHeaderString) || !authHeaderString.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedException($"Invalid authorization header format: {authHeaderString}");
            }

            // Sử dụng Substring để lấy token
            string jwtToken = authHeaderString.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();

            // Kiểm tra token có thể đọc được
            if (!tokenHandler.CanReadToken(jwtToken))
            {
                throw new UnauthorizedException("Invalid token format.");
            }

            var token = tokenHandler.ReadJwtToken(jwtToken);
            var idClaim = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

            // Kiểm tra claim id
            return idClaim?.Value ?? throw new UnauthorizedException("User ID claim is missing in the token.");
        }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
