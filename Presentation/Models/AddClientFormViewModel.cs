using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class AddClientFormViewModel
{
    [Required]
    [DataType(DataType.Text)]
    public string ClientName { get; set; } = null!;

    [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$")]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }

    [RegularExpression(@"^(?:\+46|0046|0)([ ]?\d{1,3}){2,4}$")]
    [DataType(DataType.PhoneNumber)]
    public string? Phone { get; set; }

    [DataType(DataType.ImageUrl)]
    public string? Image { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string BillingAddress { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    public string BillingReference { get; set; } = null!;

    [Required]
    [DataType(DataType.PostalCode)]
    public int PostalCode { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string CityName { get; set; } = null!;
}
