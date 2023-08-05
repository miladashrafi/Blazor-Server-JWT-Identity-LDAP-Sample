using System.ComponentModel.DataAnnotations;

namespace Backend.API.Models;

public class LoginInput
{
    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }
}