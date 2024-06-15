using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Auth;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}