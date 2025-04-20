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
        Task<ServiceResult<IEnumerable<ClientModel>>> GetAllClientsAsync();
        Task<ServiceResult> UpdateClientAsync(EditClientFormDto form);
    }

    public class ClientService(IClientRepository clientRepo, IPostalAddressService postalService, IFileHandler fileHandler, ICacheHandler<IEnumerable<ClientModel>> clientCache) : IClientService
    {
        private readonly IClientRepository _clientRepo = clientRepo;
        private readonly IPostalAddressService _postalService = postalService;
        private readonly IClientEntityFactory _clientFactory = new ClientEntityFactory();
        private readonly IFileHandler _fileHandler = fileHandler;

        private readonly ICacheHandler<IEnumerable<ClientModel>> _clientCache = clientCache;
        private const string _CACHE_KEY = "CACHE_KEY_ALL_CLIENTS";

        public async Task<ServiceResult<IEnumerable<ClientModel>>> GetAllClientsAsync()
        {
            var cached = _clientCache.Get(_CACHE_KEY);
            if (cached != null)
                return new ServiceResult<IEnumerable<ClientModel>> { Succeeded = true, StatusCode = 200, Result = cached };

            var models = await UpdateCache();
            if (models == null)
                return new ServiceResult<IEnumerable<ClientModel>> { Succeeded = false, StatusCode = 400 };

            return new ServiceResult<IEnumerable<ClientModel>> { Succeeded = true, StatusCode = 200, Result = models };
        }

        public async Task<ServiceResult<ClientModel>> GetClientByIdAsync(string clientId)
        {
            ClientModel? match = null;
            var cached = _clientCache.Get(_CACHE_KEY);

            if (cached != null)
                match = cached.FirstOrDefault(c => c.Id == clientId);

            if (match == null)
            {
                var models = await UpdateCache();

                if (models == null)
                    return new ServiceResult<ClientModel>
                    {
                        Succeeded = false,
                        StatusCode = 400
                    };

                match = models.FirstOrDefault(c => c.Id == clientId);
            }
            if (match == null)
                return new ServiceResult<ClientModel> { Succeeded = false, StatusCode = 404 };

            return new ServiceResult<ClientModel> { Succeeded = true, StatusCode = 200, Result = match };
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

            await UpdateCache();
            return result.MapTo<ServiceResult>();
        }

        public async Task<ServiceResult> UpdateClientAsync(EditClientFormDto form)
        {
            if (form == null)
                return new ServiceResult { Succeeded = false, StatusCode = 400 };
            await _postalService.AddPostalAddressAsync(new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName });
            
            var entity = _clientFactory.MapModelToEntity(form);

            var oldEntity = await GetClientByIdAsync(form.Id);
            entity.Created = oldEntity.Result.Created; // Quick dirty fix

            if (form.NewImage != null)
                entity.Image = await _fileHandler.UploadFileAsync(form.NewImage);
            else if (form.Image != null)
                entity.Image = form.Image;

                var result = await _clientRepo.UpdateAsync(entity);

            await UpdateCache();
            return result.MapTo<ServiceResult>();
        }

        public async Task<ServiceResult> DeleteClientAsync(string clientId)
        {
            var result = await _clientRepo.DeleteAsync(c => c.Id == clientId);

            await UpdateCache();
            return result.MapTo<ServiceResult<ClientModel>>();
        }

        public async Task<IEnumerable<ClientModel>>? UpdateCache()
        {
            var result = await _clientRepo.GetAllAsync(orderByDesc: false, sortBy: c => c.ClientName, filterBy: null, i => i.Billing, i => i.Billing.PostalAddress); ;

            if (!result.Succeeded)
                return null;

            _clientCache.Set(_CACHE_KEY, result.Result);
            return result.Result;
        }
    }
}
