using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class EditUserFormViewModel
{


    [Required]
    [DataType(DataType.Text)]
    public string Id { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string FirstName { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string LastName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^(?:\+46|0046|0)([ ]?\d{1,3}){2,4}$")]
    [DataType(DataType.PhoneNumber)]
    public string Phone { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string JobTitle { get; set; } = null!;

    [DataType(DataType.ImageUrl)]
    public string? Image { get; set; }

    public IFormFile? NewImage { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string Role { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string StreetAddress { get; set; } = null!;

    [Required]
    [DataType(DataType.PostalCode)]
    public int PostalCode { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string CityName { get; set; } = null!;
}
