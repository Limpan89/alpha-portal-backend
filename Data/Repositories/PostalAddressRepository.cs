using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;

public interface IPostalAddressRepository : IBaseRepository<PostalAddressEntity, PostalAddressModel> { }

public class PostalAddressRepository(DataContext context, IMemoryCache cache) : BaseRepository<PostalAddressEntity, PostalAddressModel>(context, cache), IPostalAddressRepository { }
