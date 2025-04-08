using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? ProjectName { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }

    [Column(TypeName = "Date")]
    public DateTime Created { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Budget { get; set; }

    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;

    [ForeignKey(nameof(Owner))]
    public string UserId { get; set; } = null!;
    public UserEntity Owner { get; set; } = null!;

    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; }
    public StatusEntity Status { get; set; } = null!;
}
