using Data.Contexts;
using Data.Entities;
using Data.Factories;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Data.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity, UserModel>
{
    public Task<RepositoryResult> AddAsync(UserEntity entity, string password);
}

public class UserRepository(DataContext context, UserManager<UserEntity> userManager) : BaseRepository<UserEntity, UserModel>(context, new UserModelFactory()), IUserRepository
{
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<RepositoryResult> AddAsync(UserEntity entity, string password)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            var result = await _userManager.CreateAsync(entity, password);

            if (!result.Succeeded)
                throw new Exception("Failed to sign up User.");

            return new RepositoryResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public override async Task<RepositoryResult> AddAsync(UserEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            var result = await _userManager.CreateAsync(entity);

            if (!result.Succeeded)
                throw new Exception("Failed to create User.");

            return new RepositoryResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public override async Task<RepositoryResult> UpdateAsync(UserEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            var result = await _userManager.UpdateAsync(entity);

            if (!result.Succeeded)
                throw new Exception("Failed to update User.");

            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }

    public override async Task<RepositoryResult> DeleteAsync(Expression<Func<UserEntity, bool>> expression)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(expression);
        if (entity == default)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            var result = await _userManager.DeleteAsync(entity);

            if (!result.Succeeded)
                throw new Exception("Failed to remove User.");

            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            if (ex.InnerException != null)
                Debug.WriteLine("InnerException: " + ex.InnerException);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }
}
