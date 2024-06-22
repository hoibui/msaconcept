using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityServer.Certificates;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServer.Services;

public class TokenService
{
    private readonly SigningAudienceCertificate signingAudienceCertificate;

    public TokenService()
    {
        signingAudienceCertificate = new SigningAudienceCertificate();
    }

    public string GetToken(List<Claim> authClaims)
    {
        var tokenDescriptor = GetTokenDescriptor(authClaims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);

        return token;
    }

    private SecurityTokenDescriptor GetTokenDescriptor(List<Claim> authClaims)
    {
        const int expiringDays = 7;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = DateTime.UtcNow.AddDays(expiringDays),
            SigningCredentials = signingAudienceCertificate.GetAudienceSigningKey()
        };

        return tokenDescriptor;
    }
}