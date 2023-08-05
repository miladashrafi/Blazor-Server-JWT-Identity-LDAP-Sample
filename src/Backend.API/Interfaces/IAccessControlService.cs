using System.Security.Claims;
using Backend.API.Entities;
using Backend.API.Models;

namespace Backend.API.Interfaces;

public interface IAccessControlService
{
    Task<List<string>> CreateUser(UserRegisterInput input);
    Task<UpdateUserResult> DisableUser(string id);

    Task<List<string>> CreateRole(RoleInput roleInput);

    //Task<UpdateUserResult> UpdateUserRole(string email, List<string> roleName);
    Task<List<RoleItem>> GetAllRoles();
    Task<bool> UpdateRolePermissions(string roleId, RoleInput roleInput);
    string GenerateJWTToken(Claim[] claims);
    Task<Claim[]> GetUserClaimsBy(string email);
    Task<bool> SetUserRefreshToken(string username, string refreshToken, TimeSpan expireationTime);
    string GenerateRefreshToken();
    Task<string> RefreshTokenExists(string refreshToken);
    Task<UserListVM> GetUsers(UserFilterInput filterInput);
    Task<bool> UpdateUser(string id, UpdateUserInput input);
    Task<List<ApplicationUser>> GetUsersByRoleId(string roleName);
    Task<UserVM> GetUsersById(string id);
}