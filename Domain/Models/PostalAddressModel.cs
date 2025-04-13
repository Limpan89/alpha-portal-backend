namespace Domain.Models;

public class PostalAddressModel
{
    public int PostalCode { get; set; }
    public string CityName { get; set; } = null!;
}
