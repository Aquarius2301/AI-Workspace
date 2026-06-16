using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.Settings;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Helpers;

public static class JwtHelper
{
    public static string GenerateToken(Guid userId, string email, AuthSetting authSetting)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSetting.JwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Danh sách thông tin (Claims) muốn nhúng vào trong Token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(
                JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64
            ),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(authSetting.AccessTokenMinutes),
            Issuer = authSetting.Issuer,
            Audience = authSetting.Audience,
            SigningCredentials = creds,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
