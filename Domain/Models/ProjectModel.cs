namespace Domain.Models;

public class ProjectModel
{
    public string Id { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime Created { get; set; }
    public decimal Budget { get; set; }
    public ClientModel Client { get; set; } = null!;
    public UserModel Owner { get; set; } = null!;
    public StatusModel Status { get; set; } = null!;
}
