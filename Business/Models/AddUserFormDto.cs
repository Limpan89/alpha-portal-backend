using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class AddUserFormDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? JobTitle { get; set; }
    public string? Image { get; set; }
    public string? Role { get; set; }
    public string? StreetAddress { get; set; }
    public int PostalCode { get; set; }
    public string CityName { get; set; } = null!;
}
