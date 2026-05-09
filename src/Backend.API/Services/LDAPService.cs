using Backend.API.Interfaces;
using Backend.API.Settings;
using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;

namespace Backend.API.Services;

public class LDAPService : ILdapService
{
    private readonly ILogger<LDAPService> _logger;
    private readonly string LDAPPath;

    public LDAPService(IOptions<LdapSetting> setting, ILogger<LDAPService> logger)
    {
        LDAPPath = setting.Value.LdapPath;
        _logger = logger;
    }

    public async Task<bool> Authenticate(string username, string password)
    {
        using (var connection = new LdapConnection())
        {
            try
            {
                await connection.ConnectAsync(LDAPPath, 389);
                await connection.BindAsync(username, password);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Authentivate action encountered exception {ex} for username : {username}");
                return false;
            }
        }
    }
}