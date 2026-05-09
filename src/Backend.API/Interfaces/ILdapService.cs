namespace Backend.API.Interfaces;

public interface ILdapService
{
    Task<bool> Authenticate(string username, string password);
}