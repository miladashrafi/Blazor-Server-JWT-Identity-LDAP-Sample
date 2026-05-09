using Microsoft.AspNetCore.Identity;

namespace Backend.API.Entities;

public class ApplicationUser : IdentityUser
{
    public bool Disabled { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
    public DateTime RegisterDate { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpireTime { get; set; }
    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}