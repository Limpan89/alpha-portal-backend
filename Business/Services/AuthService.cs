using Domain.Models;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Business.Models;
using Business.Handlers;
using Business.Factories;
using Microsoft.Extensions.Configuration;

namespace Business.Services;

public interface IAuthService
{
    Task<AuthResult> SignInAsync(SignInFormDto form);
    Task<ServiceResult> SignUpAsync(SignUpFormDto form);
}

public class AuthService(IUserService userService, SignInManager<UserEntity> signInManager, JwtTokenHandler tokenHandler, IUserEntityFactory entityFactory, IConfiguration configuration) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly IConfiguration _configuration = configuration;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;
    private readonly JwtTokenHandler _tokenHandler = tokenHandler;
    private readonly IUserEntityFactory _entityFactory = entityFactory;

    public async Task<AuthResult> SignInAsync(SignInFormDto form)
    {
        if (form == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, IsAdmin = false };

        var userResult = await _userService.GetUserByEmailAsync(form.Email);
        if (!userResult.Succeeded)
            return new AuthResult { Succeeded = false, StatusCode = 404, IsAdmin = false };

        string? role = null;
        var roleResult = await _userService.GetUserRoleAsync(userResult.Result!.Id);
        if (roleResult.Succeeded)
            role = roleResult.Result;

        var entity = _entityFactory.MapModelToEntity(userResult.Result!);
        var result = await _signInManager.CheckPasswordSignInAsync(entity, form.Password, false);
        var token = _tokenHandler.GenerateToken(userResult.Result!, role);
        var admin = role != null && role == "Admin";
        return new AuthResult
        {
            Succeeded = true,
            StatusCode = 200,
            Token = token,
            IsAdmin = admin,
            ApiKey = admin ? _configuration["SecretKeys:Admin"] : null,
            User = userResult.Result
        };
    }

    public async Task<ServiceResult> SignUpAsync(SignUpFormDto form)
    {
        if (form == null)
            return new ServiceResult { Succeeded = false, StatusCode = 400 };

        return await _userService.AddUserAsync(form);
    }
}
