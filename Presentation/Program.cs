
using Business.Factories;
using Business.Handlers;
using Business.Services;
using Data.Contexts;
using Data.Entities;
using Data.Factories;
using Data.Repositories;
using Data.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddMemoryCache();

        builder.Services.AddDbContext<DataContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
        builder.Services.AddIdentity<UserEntity, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
        builder.Services.AddTransient<JwtTokenHandler>();

        builder.Services.AddScoped<IClientEntityFactory, ClientEntityFactory>();
        builder.Services.AddScoped<IUserEntityFactory, UserEntityFactory>();
        builder.Services.AddScoped<IClientModelFactory, ClientModelFactory>();
        builder.Services.AddScoped<IProjectModelFactory, ProjectModelFactory>();
        builder.Services.AddScoped<IUserModelFactory, UserModelFactory>();

        builder.Services.AddScoped<IClientRepository, ClientRepository>();
        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IStatusRepository, StatusRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPostalAddressRepository, PostalAddressRepository>();

        builder.Services.AddScoped<IClientService, ClientService>();
        builder.Services.AddScoped<IProjectService, ProjectService>();
        builder.Services.AddScoped<IStatusService, StatusService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IPostalAddressService, PostalAddressService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        
        //chatgpt
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        });
        //end

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!);
            var issuer = builder.Configuration["Jwt:Issuer"]!;

            x.RequireHttpsMetadata = false; // Change
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidateIssuer = true,
                ValidIssuer = issuer,
            };
        });

        var app = builder.Build();

        await SeedData.SetRolesAsync(app);

        app.MapOpenApi();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.UseCors(x => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

        app.Run();
    }
}
