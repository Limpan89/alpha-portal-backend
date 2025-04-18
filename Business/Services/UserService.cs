using Business.Factories;
using Business.Handlers;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IUserService
{
    Task<ServiceResult> AddUserAsync(AddUserFormDto form);
    Task<ServiceResult> AddUserAsync(SignUpFormDto form, string roleName="User");
    Task<ServiceResult> DeleteUserAsync(string userId);
    Task<ServiceResult<IEnumerable<UserModel>>> GetAllUsersAsync();
    Task<ServiceResult<UserModel>> GetUserByEmailAsync(string email);
    Task<ServiceResult<UserModel>> GetUserByIdAsync(string userId);
    Task<ServiceResult> UpdateUserAsync(EditUserFormDto form);
    Task<ServiceResult<string>> GetUserRoleAsync(string userId);
}

public class UserService(IUserRepository userRepo, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManger, IPostalAddressService postalService, ICacheHandler<IEnumerable<UserModel>> userCache, ICacheHandler<IEnumerable<IdentityRole>> roleCache) : IUserService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManger = roleManger;
    private readonly IPostalAddressService _postalService = postalService;
    private readonly IUserEntityFactory _userFactory = new UserEntityFactory();
    private readonly ICacheHandler<IEnumerable<UserModel>> _userCache = userCache;
    private readonly ICacheHandler<IEnumerable<IdentityRole>> _roleCache = roleCache;

    public async Task<ServiceResult<UserModel>> GetUserByIdAsync(string userId)
    {
        var result = await _userRepo.GetAsync(u => u.Id == userId, i => i.Profile, i => i.Address, i => i.Address!.PostalAddress);
        if (result.Result != null)
        {
            var role = await GetUserRoleAsync(result.Result.Id);
            result.Result.Role = role.Result;
        }
        return result.MapTo<ServiceResult<UserModel>>();
    }

    public async Task<ServiceResult<UserModel>> GetUserByEmailAsync(string email)
    {
        var result = await _userRepo.GetAsync(u => u.Email == email, i => i.Profile, i => i.Address, i => i.Address!.PostalAddress);
        if (result.Result != null)
        {
            var role = await GetUserRoleAsync(result.Result.Id);
            result.Result.Role = role.Result;
        }
        return result.MapTo<ServiceResult<UserModel>>();
    }

    public async Task<ServiceResult<IEnumerable<UserModel>>> GetAllUsersAsync()
    {
        var result = await _userRepo.GetAllAsync(orderByDesc: false, sortBy: u => u.Profile!.LastName, filterBy: null, i => i.Profile, i => i.Address, i => i.Address!.PostalAddress);
        if (result.Result != null)
        {
            var userRoles = await GetAllUserRolesAsync();
            foreach (var u in result.Result)
                u.Role = userRoles.Result![u.Email];
        }
        return result.MapTo<ServiceResult<IEnumerable<UserModel>>>();
    }

    public async Task<ServiceResult> AddUserAsync(AddUserFormDto form)
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };

        var any = await _userRepo.ExistsAsync(u => u.Email == form.Email);
        if (any.Succeeded)
            return new ServiceResult { Succeeded = false, StatusCode = 409, Message = "A user with that email allready exists." };

        await _postalService.AddPostalAddressAsync(new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName });

        var entity = _userFactory.MapModelToEntity(form);
        var result = await _userRepo.AddAsync(entity);

        ServiceResult RoleResult;
        if (result.Succeeded)
            RoleResult = await AddUserToRoleAsync(entity.Id, form.Role);

        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> AddUserAsync(SignUpFormDto form, string roleName="User")
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };

        var any = await _userRepo.ExistsAsync(u => u.NormalizedEmail == form.Email.ToUpper());
        if (any.Succeeded)
            return new ServiceResult { Succeeded = false, StatusCode = 409, Message = "A user with that email allready exists." };

        if (await _userManager.Users.CountAsync() == 0)
            roleName = "Admin";

        var entity = _userFactory.MapSignUpToUserEntity(form);
        var result = await _userRepo.AddAsync(entity, form.Password);

        ServiceResult RoleResult;
        if (result.Succeeded)
            RoleResult = await AddUserToRoleAsync(entity.Id, roleName);

        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> UpdateUserAsync(EditUserFormDto form)
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };

        await _postalService.AddPostalAddressAsync(new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName });
        var modelResult = await _userRepo.GetAsync(u => u.Id == form.Id, i => i.Profile, i => i.Address, i => i.Address!.PostalAddress);

        if (!modelResult.Succeeded)
            return new ServiceResult { Succeeded = false, StatusCode = 404, Message = "User not found" };

        var entity = _userFactory.MapModelToEntity(form, modelResult.Result!);
        var result = await _userRepo.UpdateAsync(entity);
        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> DeleteUserAsync(string userId)
    {
        var result = await _userRepo.DeleteAsync(u => u.Id == userId);
        return result.MapTo<ServiceResult>();
    }

    public async Task<ServiceResult> AddUserToRoleAsync(string userId, string roleName)
    {
        if (!await _roleManger.RoleExistsAsync(roleName))
            return new ServiceResult { Succeeded = false, StatusCode = 404, Message = "Role not found." };

        var entity = await _userManager.FindByIdAsync(userId);
        if (entity == null)
            return new ServiceResult { Succeeded = false, StatusCode = 404, Message = "User not found." };

        var result = await _userManager.AddToRoleAsync(entity, roleName);
        return result.Succeeded
            ? new ServiceResult { Succeeded = true, StatusCode = 200 }
            : new ServiceResult { Succeeded = false, StatusCode = 500, Message = "Unable to add user to role." };
    }

    public async Task<ServiceResult> UpdateUserRoleAsync(string userId, string roleName)
    {
        if (!await _roleManger.RoleExistsAsync(roleName))
            return new ServiceResult { Succeeded = false, StatusCode = 404, Message = "Role not found." };

        var entity = await _userManager.FindByIdAsync(userId);
        if (entity == null)
            return new ServiceResult { Succeeded = false, StatusCode = 404, Message = "User not found." };

        var remove = await _userManager.RemoveFromRoleAsync(entity, roleName);
        if (!remove.Succeeded)
            return new ServiceResult { Succeeded = false, StatusCode = 500, Message = "Unable to remove user from old role." };

        var result = await _userManager.AddToRoleAsync(entity, "roleName");
        return result.Succeeded
            ? new ServiceResult { Succeeded = true, StatusCode = 200 }
            : new ServiceResult { Succeeded = false, StatusCode = 500, Message = "Unable to add user to new role." };
    }

    public async Task<ServiceResult<string>> GetUserRoleAsync(string userId)
    {
        var entity = await _userManager.FindByIdAsync(userId);
        if (entity == null)
            return new ServiceResult<string> { Succeeded = false, StatusCode = 404, Message = "User not found." };

        var roles = await _userManager.GetRolesAsync(entity);

        if (roles.Count() != 1)
            return new ServiceResult<string> { Succeeded = false, StatusCode = 500, Message = "User has multiple or no roles." };

        return new ServiceResult<string> { Succeeded = true, StatusCode = 200, Result = roles[0] };
    }

    public async Task<ServiceResult<IDictionary<string, string>>> GetAllUserRolesAsync()
    {
        var userRoles = new Dictionary<string, string>();
        string[] roles = { "User", "Admin" };
        foreach (var r in roles)
        {
            var users = await _userManager.GetUsersInRoleAsync(r);
            foreach (var u in users)
                userRoles.Add(u.Email!, r);
        }
        return new ServiceResult<IDictionary<string, string>> { Succeeded = true, StatusCode = 200, Result = userRoles };
    }
}
