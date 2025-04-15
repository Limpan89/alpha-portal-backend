using Domain.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Business.Models;
using Business.Handlers;
using Business.Factories;

namespace Business.Services;

public interface IAuthService
{
    Task<ServiceResult<string>> SignInAsync(SignInFormDto form);
    Task<ServiceResult> SignUpAsync(SignUpFormDto form);
    Task<ServiceResult> SignOut();
}

public class AuthService(IUserService userService, SignInManager<UserEntity> signInManager, JwtTokenHandler tokenHandler, IUserEntityFactory entityFactory) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly JwtTokenHandler _tokenHandler = tokenHandler;
    private readonly IUserEntityFactory _entityFactory = entityFactory;

    public async Task<ServiceResult<string>> SignInAsync(SignInFormDto form)
    {
        if (form == null)
            return new ServiceResult<string> { Succeeded = false, StatusCode = 400 };

        var userResult = await _userService.GetUserByEmailAsync(form.Email);
        if (!userResult.Succeeded)
            return new ServiceResult<string> { Succeeded = false, StatusCode = 404, Message = "User not found." };

        string? role = null;
        var roleResult = await _userService.GetUserRoleAsync(userResult.Result!.Id);
        if (roleResult.Succeeded)
            role = roleResult.Result;

        var entity = _entityFactory.MapModelToEntity(userResult.Result!);
        var result = await _signInManager.CheckPasswordSignInAsync(entity, form.Password, false);
        var token = _tokenHandler.GenerateToken(userResult.Result!, role);
        return new ServiceResult<string> { Succeeded = true, Result = token };
    }

    public async Task<ServiceResult> SignUpAsync(SignUpFormDto form)
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };

        return await _userService.AddUserAsync(form);
    }

    public async Task<ServiceResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        return new ServiceResult { Succeeded = true };
    }
}
