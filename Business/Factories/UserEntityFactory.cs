using Business.Models;
using Data.Entities;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Factories;

public interface IUserEntityFactory
{
    UserEntity MapModelToEntity(AddUserFormDto form);
    UserEntity MapModelToEntity(EditUserFormDto form, UserModel model);
    UserEntity MapModelToEntity(UserModel model);
    UserEntity MapSignUpToUserEntity(SignUpFormDto form);
}

public class UserEntityFactory : IUserEntityFactory
{
    public UserEntity MapModelToEntity(UserModel model)
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
                JobTitle = model.JobTitle,
                Image = model.Image
            },
            Address = new UserAddressEntity
            {
                UserId = model.Id,
                StreetAddress = model.StreetAddress,
                PostalCode = model.PostalAddress != null ? model.PostalAddress.PostalCode : default
            }
        };
        return entity;
    }

    public UserEntity MapModelToEntity(AddUserFormDto form)
    {
        string userId = Guid.NewGuid().ToString();
        var entity = new UserEntity
        {
            Id = userId,
            Email = form.Email,
            UserName = form.Email,

            Profile = new UserProfileEntity
            {
                UserId = userId,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Phone = form.Phone,
                JobTitle = form.JobTitle,
            },
            Address = new UserAddressEntity
            {
                UserId = userId,
                StreetAddress = form.StreetAddress,
                PostalCode = form.PostalCode
            }
        };
        return entity;
    }

    public UserEntity MapModelToEntity(EditUserFormDto form, UserModel model)
    {
        var entity = new UserEntity
        {
            Id = form.Id,
            Email = model.Email,
            UserName = model.Email,

            Profile = new UserProfileEntity
            {
                UserId = form.Id,
                FirstName = form.FirstName,
                LastName = form.LastName,
                Phone = form.Phone,
                JobTitle = form.JobTitle,
                Image = form.Image
            },
            Address = new UserAddressEntity
            {
                UserId = form.Id,
                StreetAddress = form.StreetAddress,
                PostalCode = form.PostalCode
            }
        };
        return entity;
    }

    public UserEntity MapSignUpToUserEntity(SignUpFormDto form)
    {
        string userId = Guid.NewGuid().ToString();
        var entity = new UserEntity
        {
            Id = userId,
            Email = form.Email,
            UserName = form.Email,

            Profile = new UserProfileEntity
            {
                UserId = userId,
                FirstName = form.FirstName,
                LastName = form.LastName
            },
            Address = new UserAddressEntity
            {
                UserId = userId
            }
        };
        return entity;
    }
}
