using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;

public interface IClientRepository : IBaseRepository<ClientEntity, ClientModel> { }

public class ClientRepository(DataContext context, IMemoryCache cache) : BaseRepository<ClientEntity, ClientModel>(context, cache), IClientRepository { }
