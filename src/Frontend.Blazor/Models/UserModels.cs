using System.ComponentModel.DataAnnotations;

namespace Frontend.Blazor.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email address is not valid.")]
    public string Email { get; set; } // NOTE: email will be the username, too

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}


public class UserRegisterInput: LoginModel
{
    public string Mobile { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(150)]
    public string Name { get; set; }

    [Required]
    [MinLength(2)]
    [MaxLength(150)]
    public string Family { get; set; }

    public string UserName { get; set; }

    public string[] Role { get; set; }

    [Required(ErrorMessage = "Confirm password is required.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
    public string ConfirmPassword { get; set; }
}
