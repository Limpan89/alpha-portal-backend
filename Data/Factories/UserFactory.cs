using Data.Entities;
using Domain.Models;

namespace Data.Factories;

public static class UserFactory
{
    public static UserModel Create(UserEntity entity)
    {
        var model = new UserModel
        {
            Id = entity.Id,
            Email = entity.Email!,

            FirstName = entity.Profile?.FirstName,
            LastName = entity.Profile?.LastName,
            Phone = entity.Profile?.Phone,
            Image = entity.Profile?.Image,

            StreetAddress = entity.Address?.StreetAddress,
            CityName = entity.Address?.CityName,
            PostalCode = entity.Address?.PostalCode
        };
        return model;
    }
}
