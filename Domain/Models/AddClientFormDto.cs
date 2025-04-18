using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class AddClientFormDto
{
    public string ClientName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public IFormFile? NewImage { get; set; }
    public string BillingAddress { get; set; } = null!;
    public string BillingReference { get; set; } = null!;
    public int PostalCode { get; set; }
    public string CityName { get; set; } = null!;
}
