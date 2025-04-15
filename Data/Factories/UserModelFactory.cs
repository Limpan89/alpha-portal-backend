using Data.Entities;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Data.Factories;

public interface IUserModelFactory : IModelFactory<UserEntity, UserModel> { }

public class UserModelFactory : IUserModelFactory
{
    public UserModel MapEntityToModel(UserEntity entity)
    {
        UserModel model = new UserModel
        {
            Id = entity.Id,
            Email = entity.Email!,

            FirstName = entity.Profile?.FirstName!,
            LastName = entity.Profile?.LastName!,
            Phone = entity.Profile?.Phone,
            JobTitle = entity.Profile?.JobTitle,
            Image = entity.Profile?.Image,

            StreetAddress = entity.Address?.StreetAddress,
            PostalAddress = entity.Address?.PostalAddress != null
            ? entity.Address?.PostalAddress.MapTo<PostalAddressModel>()
            : null
        };
        return model;
    }
}
