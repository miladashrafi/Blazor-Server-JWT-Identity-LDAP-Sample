using System.Security.Claims;
using System.Security.Cryptography;
using Frontend.Blazor.HttpClients;
using Frontend.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Frontend.Blazor.Code;

public class LoginService
{
    private const string AccessToken = nameof(AccessToken);
    private const string RefreshToken = nameof(RefreshToken);

    private readonly ProtectedLocalStorage _localStorage;
    private readonly NavigationManager _navigation;
    private readonly IConfiguration _configuration;
    private readonly IBackendApiHttpClient _backendApiHttpClient;

    public LoginService(ProtectedLocalStorage localStorage, NavigationManager navigation, IConfiguration configuration, IBackendApiHttpClient backendApiHttpClient)
    {
        _localStorage = localStorage;
        _navigation = navigation;
        _configuration = configuration;
        _backendApiHttpClient = backendApiHttpClient;
    }

    public async Task<bool> LoginAsync(LoginModel model)
    {
        var response = await _backendApiHttpClient.LoginUserAsync(model);
        if (string.IsNullOrEmpty(response?.Result?.JwtToken)) 
            return false;
        
        await _localStorage.SetAsync(AccessToken, response.Result.JwtToken);
        await _localStorage.SetAsync(RefreshToken, response.Result.RefreshToken);

        return true;
    }


    public async Task<List<Claim>> GetLoginInfoAsync()
    {
        var emptyResult = new List<Claim>();
        ProtectedBrowserStorageResult<string> accessToken;
        ProtectedBrowserStorageResult<string> refreshToken;
        try
        {
            accessToken = await _localStorage.GetAsync<string>(AccessToken);
            refreshToken = await _localStorage.GetAsync<string>(RefreshToken);
        }
        catch (CryptographicException)
        {
            await LogoutAsync();
            return emptyResult;
        }

        if (accessToken.Success is false || accessToken.Value == default) 
            return emptyResult;
        
        var claims = JwtTokenHelper.ValidateDecodeToken(accessToken.Value, _configuration);
            
        if (claims.Count != 0) 
            return claims;
            
        if (refreshToken.Value != default)
        {
            var response = await _backendApiHttpClient.RefreshTokenAsync(refreshToken.Value);
            if (string.IsNullOrWhiteSpace(response?.Result?.JwtToken) is false)
            {
                await _localStorage.SetAsync(AccessToken, response.Result.JwtToken);
                await _localStorage.SetAsync(RefreshToken, response.Result.RefreshToken);
                claims = JwtTokenHelper.ValidateDecodeToken(response.Result.JwtToken, _configuration);
                return claims;
            }
            else
            {
                await LogoutAsync();
            }
        }
        else
        {
            await LogoutAsync();
        }
        return claims;
    }

    public async Task LogoutAsync()
    {
        await RemoveAuthDataFromStorageAsync();
        _navigation.NavigateTo("/", true);
    }

    private async Task RemoveAuthDataFromStorageAsync()
    {
        await _localStorage.DeleteAsync(AccessToken);
        await _localStorage.DeleteAsync(RefreshToken);
    }
}