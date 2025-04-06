using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class AppUserAddressEntity
{
    [Key, ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public AppUserEntity User { get; set; } = null!;

    public string StreetName { get; set; } = null!;
    public string? StreetNumber { get; set; }

    [ForeignKey(nameof(PostalCodeId))]
    public string PostalCodeId { get; set; } = null!;
    public PostalCodeEntity PostalCode { get; set; }
}
