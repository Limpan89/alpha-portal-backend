using Data.Entities;
using Domain.Extensions;
using Domain.Models;

namespace Data.Factories;

public interface IClientModelFactory : IModelFactory<ClientEntity, ClientModel> { }

public class ClientModelFactory : IClientModelFactory
{
    public ClientModel MapEntityToModel(ClientEntity entity)
    {
        var model = new ClientModel
        {
            Id = entity.Id,
            ClientName = entity.ClientName,
            Email = entity.Email,
            Phone = entity.Phone,
            Image = entity.Image,
            BillingAddress = entity.Billing!.BillingAddress,
            BillingReference = entity.Billing!.BillingReference,
            PostalAddress = entity.Billing.PostalAddress.MapTo<PostalAddressModel>()
        };
        return model;
    }
}
