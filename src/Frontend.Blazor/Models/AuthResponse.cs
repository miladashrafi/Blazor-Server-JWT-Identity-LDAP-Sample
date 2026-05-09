namespace Frontend.Blazor.Models;

public class AuthResponse
{
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
}