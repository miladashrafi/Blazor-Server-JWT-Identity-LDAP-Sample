namespace Backend.API.Models;

public class UserFilterInput
{
    public string Email { get; set; }
    public string Role { get; set; }
    public DateTime? CreationTime { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
    public int PageNumber { get; set; }
    public int RowCount { get; set; }
}