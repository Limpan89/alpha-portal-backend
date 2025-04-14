using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Data.Entities
{
    [Index(nameof(BillingReference), IsUnique = true)]
    public class ClientBillingEntity
    {
        [Key, ForeignKey(nameof(Client))]
        public string ClientId { get; set; } = null!;
        public ClientEntity Client { get; set; } = null!;
        public string BillingAddress { get; set; } = null!;
        public string BillingReference { get; set; } = null!;

        [ForeignKey(nameof(PostalAddress))]
        public int PostalCode { get; set; }
        public virtual PostalAddressEntity PostalAddress { get; set; } = null!;
    }
}
