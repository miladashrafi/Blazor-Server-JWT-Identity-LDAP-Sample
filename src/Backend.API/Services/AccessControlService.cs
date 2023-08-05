using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Backend.API.Entities;
using Backend.API.Interfaces;
using Backend.API.Models;
using Backend.API.Permissions;
using Backend.API.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.API.Services;

public class AccessControlService : IAccessControlService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AccessControlService> _logger;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccessControlService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
        IOptions<JwtSettings> jwtSettings, ILogger<AccessControlService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    public async Task<Claim[]> GetUserClaimsBy(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.Disabled) return null;

        var roles = await _userManager.GetRolesAsync(user);
        if (roles == null) return null;

        var userRole = roles.Select(x => new Claim(ClaimTypes.Role, x));
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roleClaims = await _roleManager.FindByNameAsync(roles.FirstOrDefault())
            .ContinueWith(x => _roleManager.GetClaimsAsync(x.Result)).Unwrap();
        return new Claim[]
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Sid, user.Id)
        }.Union(userRole).Union(userClaims).Union(roleClaims).ToArray();
    }

    public async Task<List<string>> CreateUser(UserRegisterInput input)
    {
        var user = new ApplicationUser
        {
            Email = input.Email,
            UserName = input.Email,
            Name = input.Name,
            Family = input.Family,
            RegisterDate = DateTime.Now,
            SecurityStamp = Guid.NewGuid().ToString(),
            PhoneNumber = input.Mobile
        };
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, input.Password);
        
        var result = await _userManager.CreateAsync(user);
        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRolesAsync(user, input.Role);
            if (roleResult.Succeeded) return new List<string>();

            return roleResult.Errors.Select(x => x.Description).ToList();
        }

        return result.Errors.Select(x => x.Description).ToList();
    }

    public async Task<UpdateUserResult> DisableUser(string id)
    {
        ApplicationUser user = null;
        var result = new UpdateUserResult { Succeeded = false, UpdatedUser = user };

        if (!string.IsNullOrEmpty(id))
            user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return result;

        user.Disabled = true;
        var identityResult = await _userManager.UpdateAsync(user);
        if (identityResult.Succeeded)
        {
            result.Succeeded = identityResult.Succeeded;
            result.UpdatedUser = user;
        }

        return result;
    }


    public async Task<string> RefreshTokenExists(string refreshToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x =>
            x.RefreshToken == refreshToken && x.RefreshTokenExpireTime > DateTime.Now);
        return user?.Email;
    }


    public async Task<List<string>> CreateRole(RoleInput roleInput)
    {
        try
        {
            var role = new ApplicationRole(roleInput.Name);
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                var errors = new List<string>();
                var permissions = new PermissionList();
                foreach (var claim in roleInput.Permissions)
                    if (permissions.GetPermissionByKey(claim.Value) != null)
                    {
                        if (claim.IsChecked)
                        {
                            var claimResult =
                                await _roleManager.AddClaimAsync(role,
                                    new Claim(ClaimConstants.PermissionClaimName, claim.Value));
                            if (!claimResult.Succeeded)
                                errors.AddRange(claimResult.Errors.Select(x => x.Description).ToList());
                        }
                        else
                        {
                            var claimResult =
                                await _roleManager.RemoveClaimAsync(role,
                                    new Claim(ClaimConstants.PermissionClaimName, claim.Value));
                            if (!claimResult.Succeeded)
                                errors.AddRange(claimResult.Errors.Select(x => x.Description).ToList());
                        }
                    }
                    else
                    {
                        errors.Add($"there is no such permission:{claim}");
                    }

                return errors;
            }

            return new List<string>(result.Errors.Select(x => x.Description).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception occured on create Role {ex}");
            return new List<string>();
        }
    }

    public async Task<List<RoleItem>> GetAllRoles()
    {
        try
        {
            var result = new List<RoleItem>();
            var roles = _roleManager.Roles.ToList();
            foreach (var role in roles)
            {
                var cl = await _roleManager.GetClaimsAsync(role);
                var permissions =
                    cl.Where(x => x.Type == ClaimConstants.PermissionClaimName).Select(x => x.Value).ToList();

                var permissionList = new PermissionList();
                foreach (var permission in permissions) permissionList.SetPermissionEnable(permission);

                result.Add(new RoleItem(role.Id, role.Name, permissionList));
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($" exception in getAllRoles {ex}");
            return null;
        }
    }

    public async Task<bool> SetUserRefreshToken(string username, string refreshToken, TimeSpan expirationTime)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(username);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireTime = DateTime.Now.Add(expirationTime);
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError($"exception on refresh token {ex}");
            throw;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public async Task<bool> UpdateRolePermissions(string roleId, RoleInput roleInput)
    {
        var role = await _roleManager.FindByIdAsync(roleId);
        if (role == null) return false;

        if (!string.IsNullOrEmpty(roleInput.Name) &&
            !role.Name.Equals(roleInput.Name, StringComparison.InvariantCultureIgnoreCase))
        {
            role.Name = roleInput.Name;
            await _roleManager.UpdateAsync(role);
        }

        var roleClaims = (await _roleManager.GetClaimsAsync(role)).Where(x => x.Type == ClaimConstants.PermissionClaimName);
        var addedPermissions = roleInput.Permissions.Where(p => p.IsChecked).Select(p => p.Value);
        var removedPermissions = roleInput.Permissions.Where(p => !p.IsChecked).Select(p => p.Value);
        var removedClaims = roleClaims.Where(c => removedPermissions.Contains(c.Value));

        foreach (var claim in removedClaims) await _roleManager.RemoveClaimAsync(role, claim);

        foreach (var claim in addedPermissions)
            await _roleManager.AddClaimAsync(role, new Claim(ClaimConstants.PermissionClaimName, claim));

        return true;
    }

    public async Task<UserListVM> GetUsers(UserFilterInput filterInput)
    {
        var items = await _userManager.Users.Include(u => u.UserRoles).OrderBy(x => x.Email)
            .Where(x =>
                (string.IsNullOrEmpty(filterInput.Email) ||
                 x.Email.ToLower().Contains(filterInput.Email.ToLower()))
                && (string.IsNullOrEmpty(filterInput.Role) ||
                    x.UserRoles.Count(y => y.Role.Name.Contains(filterInput.Role)) > 0)
                && (string.IsNullOrEmpty(filterInput.Name) || x.Name.Contains(filterInput.Name))
                && (string.IsNullOrEmpty(filterInput.Family) || x.Name.Contains(filterInput.Family))
                && (filterInput.CreationTime == null || x.RegisterDate > filterInput.CreationTime)
                && !x.Disabled
            )
            .Skip(filterInput.RowCount * (filterInput.PageNumber - 1)).Take(filterInput.RowCount).Select(x =>
                new UserVM
                {
                    Id = x.Id,
                    Email = x.Email,
                    Mobile = x.PhoneNumber,
                    Username = x.UserName,
                    CreationDate = x.RegisterDate,
                    Name = x.Name,
                    Family = x.Family,
                    Role = x.UserRoles.Select(y => y.Role.Name).ToList()
                }).ToListAsync();

        var totalRows = _userManager.Users.Include(u => u.UserRoles).OrderBy(x => x.Email)
            .Count(x =>
                (string.IsNullOrEmpty(filterInput.Email) ||
                 x.Email.ToLower().Contains(filterInput.Email.ToLower()))
                && (string.IsNullOrEmpty(filterInput.Role) ||
                    x.UserRoles.Count(y => y.Role.Name.Contains(filterInput.Role)) > 0)
                && (string.IsNullOrEmpty(filterInput.Name) || x.Name.Contains(filterInput.Name))
                && (string.IsNullOrEmpty(filterInput.Family) || x.Name.Contains(filterInput.Family))
                && (filterInput.CreationTime == null || x.RegisterDate > filterInput.CreationTime)
                && !x.Disabled
            );
        return new UserListVM
        {
            Users = items,
            TotalRow = totalRows
        };
    }

    public async Task<bool> UpdateUser(string id, UpdateUserInput input)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null) return false;

        user.Name = input.Name;
        user.Family = input.Family;
        user.PhoneNumber = input.Mobile;
        if (user.UserName != user.Email) //for auto-correction
            user.UserName = user.Email;

        var result = await _userManager.UpdateAsync(user);
        var roleResult = await UpdateUserRole(id, input.Role);
        return result.Succeeded && roleResult.Succeeded;
    }

    public async Task<List<ApplicationUser>> GetUsersByRoleId(string roleId)
    {
        return await _userManager.Users.Where(x => !x.Disabled && x.UserRoles.Count(y => y.Role.Id == roleId) > 0)
            .ToListAsync();
    }

    public async Task<UserVM> GetUsersById(string id)
    {
        /*
                    var temp = _userManager.Users.FirstOrDefault(x => x.Id == id);
                    var user = await _userManager.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(x=>x.Id == id);
                    var roles = user.UserRoles?.Select(x => x.Role?.Name).ToList();
        */

        var item = await _userManager.Users.Include(u => u.UserRoles).OrderBy(x => x.Email)
            .Where(x =>
                x.Id == id
                && !x.Disabled
            ).Select(x =>
                new UserVM
                {
                    Id = x.Id,
                    Email = x.Email,
                    Mobile = x.PhoneNumber,
                    Username = x.UserName,
                    CreationDate = x.RegisterDate,
                    Name = x.Name,
                    Family = x.Family,
                    Role = x.UserRoles.Select(y => y.Role.Name).ToList()
                }).FirstOrDefaultAsync();


        return item;
    }

    public string GenerateJWTToken(Claim[] claims)
    {
        var finalClaims = new List<Claim>();
        if (claims == null) return null;
        foreach (var claim in claims)
        {
            var type = claim.Type;
            if (claim.Type.Contains("/")) type = claim.Type.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
            finalClaims.Add(new Claim(type, claim.Value));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Issuer,
            Subject = new ClaimsIdentity(finalClaims),
            Expires = DateTime.Now.AddMinutes(_jwtSettings.JWTExpirationTime),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret),
                SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<UpdateUserResult> UpdateUserRole(string id, List<string> newRoles)
    {
        var result = new UpdateUserResult { UpdatedUser = null, Succeeded = false };
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user != null)
        {
            result.UpdatedUser = user;
            var roles = await _userManager.GetRolesAsync(user);
            var addedRoles = newRoles.Except(roles);
            var removedRoles = roles.Except(newRoles);
            if (roles.Count > 0) await _userManager.RemoveFromRolesAsync(user, removedRoles);

            var updateResult = await _userManager.AddToRolesAsync(user, addedRoles);
            if (updateResult.Succeeded)
            {
                result.Succeeded = updateResult.Succeeded;
                return result;
            }
        }

        return result;
    }
}