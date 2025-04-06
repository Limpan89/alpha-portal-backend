using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class PostalCodeEntity
{
    [Key]
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
}