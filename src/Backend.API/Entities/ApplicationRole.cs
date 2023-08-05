using Microsoft.AspNetCore.Identity;

namespace Backend.API.Entities;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole(string name) : base(name)
    {
    }

    public ICollection<ApplicationUserRole> UserRoles { get; set; }
}