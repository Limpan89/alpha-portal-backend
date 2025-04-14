using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class EditUserFormDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? JobTitle { get; set; }
    public string? Image { get; set; }
    public string Role { get; set; } = null!;
    public string? StreetAddress { get; set; }
    public int PostalCode { get; set; }
    public string CityName { get; set; } = null!;
}
