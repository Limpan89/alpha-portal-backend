using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class EditProjectFormViewModel
{
    [Required]
    [DataType(DataType.Text)]
    public string Id { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string ProjectName { get; set; } = null!;

    [DataType(DataType.Text)]
    public string? Description { get; set; }

    [DataType(DataType.ImageUrl)]
    public string? Image { get; set; }

    public IFormFile? NewImage { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime EndDate { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal Budget { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string ClientId { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string UserId { get; set; } = null!;

    [Required]
    public int StatusId { get; set; }
}
