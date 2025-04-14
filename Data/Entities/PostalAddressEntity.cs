using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class PostalAddressEntity
    {
        [Key]
        public int PostalCode { get; set; }
        public string CityName { get; set; } = null!;

        public virtual ICollection<UserAddressEntity> Users { get; set; } = [];
        public virtual ICollection<ClientBillingEntity> Clients { get; set; } = [];
    }
}
