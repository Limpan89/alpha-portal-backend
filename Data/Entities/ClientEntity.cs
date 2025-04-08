using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ClientEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ClientName { get; set; } = null!;
    public string? Image { get; set; }
}
