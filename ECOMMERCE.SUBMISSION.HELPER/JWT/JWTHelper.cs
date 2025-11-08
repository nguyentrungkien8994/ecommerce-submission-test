using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace ECOMMERCE.SUBMISSION.HELPER;

public class JWTHelper
{
    /// <summary>
    /// Generate jwt token
    /// </summary>
    /// <param name="secretKey"></param>
    /// <param name="issuer"></param>
    /// <param name="audience"></param>
    /// <param name="expired"></param>
    /// <param name="roles"></param>
    /// <param name="sub"></param>
    /// <param name="givenName"></param>
    /// <param name="birthDate"></param>
    /// <returns></returns>
    public static string GenerateJwtToken(string secretKey, string issuer, string audience, DateTime expired, string roles = "user", string sub = "ecommerce-submission",string email="", string givenName = "user", long birthDate = 0)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, sub),
            new Claim(JwtRegisteredClaimNames.GivenName, givenName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, roles),
            new Claim(JwtRegisteredClaimNames.Birthdate, birthDate.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            // Add additional claims as needed
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expired, // Set token expiration time
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Parser JWT token
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public static JwtSecurityToken ParserAccessToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        return jwtSecurityToken;
    }

    public static string GetAccessTokenFromContext(IHttpContextAccessor _httpContextAccessor)
    {
        string? accessToken = _httpContextAccessor?.HttpContext?.Request?.Headers["Authorization"];

        if (string.IsNullOrEmpty(accessToken))
        {
            accessToken = _httpContextAccessor?.HttpContext?.Request?.Query["access_token"].ToString();
        }

        accessToken = accessToken?.Replace("Bearer", "").Trim();
        return accessToken ?? string.Empty;
    }
}
