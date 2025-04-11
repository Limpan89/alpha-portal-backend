using Data.Entities;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Data.Factories;

public static class UserFactory
{
    public static UserModel MapEntityToModel(UserEntity entity)
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

    public static UserEntity MapModelToEntity(UserModel model)
    {
        var entity = new UserEntity
        {
            Id = model.Id,
            Email = model.Email,
            UserName = model.Email,

            Profile = new UserProfileEntity
            {
                UserId = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Phone = model.Phone,
                Image = model.Image
            },

            Address = new UserAddressEntity
            {
                UserId = model.Id,
                StreetAddress = model.StreetAddress,
                CityName = model.CityName,
                PostalCode = model.PostalCode
            }
        };
        return entity;
    }
}
