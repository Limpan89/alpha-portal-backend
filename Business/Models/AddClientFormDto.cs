using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Business.Models;

public class AddClientFormDto
{
    public string ClientName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Image { get; set; }
    public string BillingAddress { get; set; } = null!;
    public string BillingReference { get; set; } = null!;
    public int PostalCode { get; set; }
    public string CityName { get; set; } = null!;
}
