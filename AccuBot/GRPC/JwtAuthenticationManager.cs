using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Proto.API;
using Proto.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace AccuBot.GRPC;

public class JwtAuthenticationManager
{
    public const string JWT_TOKEN_KEY = "Accumulate@1234567890";
    private const int JWT_TOKEN_VALIDITY = 30;
    
    public static AuthenticationReply Authenticate(AuthenticationRequest authenticationRequest)
    {
        if (authenticationRequest.Username != "admin" || authenticationRequest.Password != "admin") return null;

        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.ASCII.GetBytes(JWT_TOKEN_KEY);
        var tokenExpiryDateTime = DateTime.UtcNow.AddMinutes(JWT_TOKEN_VALIDITY);
        var securityTokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new List<Claim>()
            {
                new Claim("username", authenticationRequest.Username),
                new Claim(ClaimTypes.Role, "Administrator")
            }),
            Expires = tokenExpiryDateTime,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
        var token = jwtSecurityTokenHandler.WriteToken(securityToken);

        return new AuthenticationReply()
        {
            AccessToken = token,
            ExpiresIn = (int)tokenExpiryDateTime.Subtract(DateTime.UtcNow).TotalSeconds
        };
    }
}