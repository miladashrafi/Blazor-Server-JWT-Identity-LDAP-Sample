using System.ComponentModel.DataAnnotations;
using Backend.API.Permissions;

namespace Backend.API.Models;

public class UserRegisterInput
{
    [Required] [EmailAddress] public string Email { get; set; }

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
    
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
    
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    public string[] Role { get; set; }
}

public class RoleItem
{
    public RoleItem(string id, string name, PermissionList permissionList)
    {
        Id = id;
        Name = name;
        _permissionList = permissionList;
    }

    public string Id { get; private set; }
    public string Name { get; private set; }


    private PermissionList _permissionList { get; }
    public ICollection<Permission> Permissions => _permissionList.Permissions;
}

public class PermissionInput
{
    public string Value { get; set; }
    public bool IsChecked { get; set; }
}

public class RoleInput
{
    public string Name { get; set; }
    public List<PermissionInput> Permissions { get; set; }
}

public class UserDisableInput : IValidatableObject
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Id { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(UserName) && string.IsNullOrEmpty(Id))
            yield return new ValidationResult("At least one of the filter inputs should have a value");
    }
}