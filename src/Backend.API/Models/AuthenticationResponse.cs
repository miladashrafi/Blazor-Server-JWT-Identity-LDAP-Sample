namespace Backend.API.Models;

public class AuthenticationResponse
{
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
}