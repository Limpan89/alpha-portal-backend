using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;

namespace Business.Services;

public interface IPostalAddressService
{
    Task<ServiceResult> AnyPostalAddressAsync(int postalCode);
    Task<ServiceResult> AddPostalAddressAsync(PostalAddressEntity entity);
    Task<ServiceResult<PostalAddressModel>> GetPostalAddressByIdAsync(int postalCode);
}

public class PostalAddressService(IPostalAddressRepository postalRepo) : IPostalAddressService
{
    private readonly IPostalAddressRepository _postalRepo = postalRepo;

    public async Task<ServiceResult> AnyPostalAddressAsync(int postalCode)
    {
        var result = await _postalRepo.ExistsAsync(p => p.PostalCode == postalCode);
        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult<PostalAddressModel>> GetPostalAddressByIdAsync(int postalCode)
    {
        var result = await _postalRepo.GetAsync(p => p.PostalCode == postalCode);
        return result.MapTo<ServiceResult<PostalAddressModel>>();
    }

    public async Task<ServiceResult> AddPostalAddressAsync(PostalAddressEntity entity)
    {
        var result = await _postalRepo.AddAsync(entity);
        return result.MapTo<ServiceResult>();
    }
}
