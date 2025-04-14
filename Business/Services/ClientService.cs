using Business.Factories;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services
{
    public interface IClientService
    {
        Task<ServiceResult> AddClientAsync(AddClientFormDto form);
        Task<ServiceResult> DeleteClientAsync(string clientId);
        Task<ServiceResult<ClientModel>> GetClientByIdAsync(string clientId);
        Task<ServiceResult<IEnumerable<ClientModel>>> GetClientsAsync();
        Task<ServiceResult> UpdateClientAsync(EditClientFormDto form);
    }

    public class ClientService(IClientRepository clientRepo, IPostalAddressService postalService) : IClientService
    {
        private readonly IClientRepository _clientRepo = clientRepo;
        private readonly IPostalAddressService _postalService = postalService;
        private readonly IClientEntityFactory _clientFactory = new ClientEntityFactory();

        public async Task<ServiceResult<IEnumerable<ClientModel>>> GetClientsAsync()
        {
            var result = await _clientRepo.GetAllAsync(orderByDesc: false, sortBy: c => c.ClientName, filterBy: null, i => i.Billing!.PostalAddress);
            return result.MapTo<ServiceResult<IEnumerable<ClientModel>>>();
        }

        public async Task<ServiceResult<ClientModel>> GetClientByIdAsync(string clientId)
        {
            var result = await _clientRepo.GetAsync(c => c.Id == clientId, i => i.Billing!.PostalAddress);
            return result.MapTo<ServiceResult<ClientModel>>();
        }

        public async Task<ServiceResult> AddClientAsync(AddClientFormDto form)
        {
            if (form == null)
                return new ServiceResult { Succeeded = false, StatusCode = 400 };
            var any = await _postalService.AnyPostalAddressAsync(form.PostalCode);
            var entity = _clientFactory.MapModelToEntity(form);

            if (!any.Succeeded)
                entity.Billing.PostalAddress = new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName };

            var result = await _clientRepo.AddAsync(entity);
            return result.MapTo<ServiceResult>();
        }

        public async Task<ServiceResult> UpdateClientAsync(EditClientFormDto form)
        {
            if (form == null)
                return new ServiceResult { Succeeded = false, StatusCode = 400 };
            var any = await _postalService.AnyPostalAddressAsync(form.PostalCode);
            var entity = _clientFactory.MapModelToEntity(form);

            if (!any.Succeeded)
                entity.Billing.PostalAddress = new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName };

            var result = await _clientRepo.UpdateAsync(entity);
            return result.MapTo<ServiceResult>();
        }

        public async Task<ServiceResult> DeleteClientAsync(string clientId)
        {
            var result = await _clientRepo.DeleteAsync(c => c.Id == clientId);
            return result.MapTo<ServiceResult<ClientModel>>();
        }
    }
}
