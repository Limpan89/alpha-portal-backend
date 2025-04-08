namespace Domain.Models;

public class UserModel
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Image { get; set; }
    public string? StreetAddress { get; set; }
    public string? CityName { get; set; }
    public string? PostalCode { get; set; }
}
