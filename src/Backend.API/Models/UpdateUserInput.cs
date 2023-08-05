using System.ComponentModel.DataAnnotations;
using Backend.API.Entities;

namespace Backend.API.Models;

public class UpdateUserInput
{
    public string Mobile { get; set; }

    [Required]
    [MaxLength(150)]
    [MinLength(2)]

    public string Name { get; set; }

    [Required]
    [MaxLength(150)]
    [MinLength(2)]
    public string Family { get; set; }

    public List<string> Role { get; set; }
}

public class UpdateUserResult
{
    public ApplicationUser UpdatedUser { get; set; }
    public bool Succeeded { get; set; }
}