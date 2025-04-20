using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class AddUserFormDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public IFormFile? NewImage { get; set; }
    public string Role { get; set; } = null!;
    public string StreetAddress { get; set; } = null!;
    public int PostalCode { get; set; }
    public string CityName { get; set; } = null!;
}
