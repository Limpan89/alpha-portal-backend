using Business.Models;
using Data.Entities;
using Domain.Models;

namespace Business.Factories;

public interface IClientEntityFactory : IEntityFactory<ClientEntity, ClientModel, AddClientFormDto, EditClientFormDto> { }

public class ClientEntityFactory : IClientEntityFactory
{
    public ClientEntity MapModelToEntity(ClientModel model)
    {
        var entity = new ClientEntity
        {
            Id = model.Id,
            ClientName = model.ClientName,
            Email = model.Email,
            Phone = model.Phone,
            Image = model.Image,
            Billing = new ClientBillingEntity
            {
                ClientId = model.Id,
                BillingAddress = model.BillingAddress,
                BillingReference = model.BillingReference,
                PostalCode = model.PostalAddress.PostalCode
            }
        };
        return entity;
    }

    public ClientEntity MapModelToEntity(AddClientFormDto form)
    {
        string clientId = Guid.NewGuid().ToString();
        var entity = new ClientEntity
        {
            Id = clientId,
            ClientName = form.ClientName,
            Email = form.Email,
            Phone = form.Phone,
            Image = form.Image,
            Billing = new ClientBillingEntity
            {
                ClientId = clientId,
                BillingAddress = form.BillingAddress,
                BillingReference = form.BillingReference,
                PostalCode = form.PostalCode
            }
        };
        return entity;
    }

    public ClientEntity MapModelToEntity(EditClientFormDto form)
    {
        var entity = new ClientEntity
        {
            Id = form.Id,
            ClientName = form.ClientName,
            Email = form.Email,
            Phone = form.Phone,
            Image = form.Image,
            Billing = new ClientBillingEntity
            {
                ClientId = form.Id,
                BillingAddress = form.BillingAddress,
                BillingReference = form.BillingReference,
                PostalCode = form.PostalCode
            }
        };
        return entity;
    }
}
