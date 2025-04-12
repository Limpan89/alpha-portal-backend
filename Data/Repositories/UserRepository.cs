using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Data.Repositories;

public interface IUserRepository : IBaseRepository<UserEntity, UserModel>
{
    public Task<RepositoryResult> AddAsync(UserEntity entity, string password);
}

public class UserRepository(DataContext context, IMemoryCache cache, UserManager<UserEntity> userManager) : BaseRepository<UserEntity, UserModel>(context, cache), IUserRepository
{
    private readonly UserManager<UserEntity> _userManager = userManager;

    public async Task<RepositoryResult> AddAsync(UserEntity entity, string password)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            int users = _userManager.Users.Count();
            var result = await _userManager.CreateAsync(entity);

            if (!result.Succeeded)
                throw new Exception("Failed to create User.");

            if (users < 1)
                await _userManager.AddToRoleAsync(entity, "Admin");
            else
                await _userManager.AddToRoleAsync(entity, "User");

            ClearCache();
            return new RepositoryResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Message = ex.Message };
        }
    }
}
