using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

// Create Seperate Entity for PostalAdress (Normalization n' stuff)

namespace Data.Entities;

public class UserAddressEntity
{
    [Key, ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
    public string? StreetAddress { get; set; }
    public string? PostalCode { get; set; }
    public string? CityName { get; set; }
}
