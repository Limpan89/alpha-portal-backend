using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class PostalAddressEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PostalCode { get; set; }
        public string CityName { get; set; } = null!;

        public virtual ICollection<UserAddressEntity> Users { get; set; } = [];
        public virtual ICollection<ClientBillingEntity> Clients { get; set; } = [];
    }
}
