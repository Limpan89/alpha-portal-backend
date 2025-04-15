namespace Domain.Models;

public class UserModel
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public string? Image { get; set; }
    public string? StreetAddress { get; set; }
    public string? Role { get; set; }
    public PostalAddressModel? PostalAddress { get; set; }
}
