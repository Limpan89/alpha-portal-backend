using Business.Factories;
using Business.Handlers;
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

    public class ClientService(IClientRepository clientRepo, IPostalAddressService postalService, IFileHandler fileHandler) : IClientService
    {
        private readonly IClientRepository _clientRepo = clientRepo;
        private readonly IPostalAddressService _postalService = postalService;
        private readonly IClientEntityFactory _clientFactory = new ClientEntityFactory();
        private readonly IFileHandler _fileHandler = fileHandler;

        public async Task<ServiceResult<IEnumerable<ClientModel>>> GetClientsAsync()
        {
            var result = await _clientRepo.GetAllAsync(orderByDesc: false, sortBy: c => c.ClientName, filterBy: null, i => i.Billing, i => i.Billing.PostalAddress);
            return result.MapTo<ServiceResult<IEnumerable<ClientModel>>>();
        }

        public async Task<ServiceResult<ClientModel>> GetClientByIdAsync(string clientId)
        {
            var result = await _clientRepo.GetAsync(c => c.Id == clientId, i => i.Billing, i => i.Billing!.PostalAddress);
            return result.MapTo<ServiceResult<ClientModel>>();
        }

        public async Task<ServiceResult> AddClientAsync(AddClientFormDto form)
        {
            if (form == null)
                return new ServiceResult { Succeeded = false, StatusCode = 400 };
            await _postalService.AddPostalAddressAsync(new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName });

            var entity = _clientFactory.MapModelToEntity(form);
            if (form.NewImage != null)
                entity.Image = await _fileHandler.UploadFileAsync(form.NewImage);

            var result = await _clientRepo.AddAsync(entity);
            return result.MapTo<ServiceResult>();
        }

        public async Task<ServiceResult> UpdateClientAsync(EditClientFormDto form)
        {
            if (form == null)
                return new ServiceResult { Succeeded = false, StatusCode = 400 };
            await _postalService.AddPostalAddressAsync(new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName });
            
            var entity = _clientFactory.MapModelToEntity(form);

            if (form.NewImage != null)
                entity.Image = await _fileHandler.UploadFileAsync(form.NewImage);
            else if (form.Image != null)
                entity.Image = form.Image;

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
