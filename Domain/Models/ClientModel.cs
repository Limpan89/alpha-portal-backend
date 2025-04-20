namespace Domain.Models
{
    public class ClientModel
    {
        public string Id { get; set; } = null!;
        public string ClientName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Image { get; set; }
        public DateTime Created { get; set; }
        public string BillingAddress { get; set; } = null!;
        public string BillingReference { get; set; } = null!;
        public PostalAddressModel PostalAddress { get; set; } = null!;
    }
}
