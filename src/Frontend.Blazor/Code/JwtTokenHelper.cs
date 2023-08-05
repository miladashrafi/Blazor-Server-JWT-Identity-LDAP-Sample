using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Frontend.Blazor.Code
{
    public static class JwtTokenHelper
    {
        public static List<Claim> ValidateDecodeToken(string token, IConfiguration configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ValidIssuer = configuration.GetValue<string>("JWTSettings:ValidIssuer"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("JWTSettings:Secret")))
                }, out var validatedToken);
            }
            catch
            {
                return new List<Claim>();
            }

            var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return securityToken?.Claims.ToList();
        }
    }
}
