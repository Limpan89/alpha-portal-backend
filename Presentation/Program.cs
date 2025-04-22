
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
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddMemoryCache();

        builder.Services.AddSwaggerGen(o =>
        {
            o.EnableAnnotations();
            o.ExampleFilters();
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v 1.0",
                Title = "Alpha Portal API Documenatation",
                Description = "Standard documentation for Alpha Portal API."
            });
            var apiAdminScheme = new OpenApiSecurityScheme
            {
                Name = "X-ADM-API-KEY",
                Description = "Admin Api-Key Required",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "ApiKeyScheme",
                Reference = new OpenApiReference
                {
                    Id = "AdminApiKey",
                    Type = ReferenceType.SecurityScheme
                }
            };
            o.AddSecurityDefinition("AdminApiKey", apiAdminScheme);
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { apiAdminScheme, new List<string>() }
            });
        });

        builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

        builder.Services.AddDbContext<DataContext>(e => e.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));
        builder.Services.AddIdentity<UserEntity, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

        var blobConnString = builder.Configuration.GetConnectionString("AzureBlobStorage")!;
        var containerName = "images";
        builder.Services.AddScoped<IFileHandler>(_ => new AzureFileHandler(blobConnString, containerName));

        /*
        var localFilePath = Path.Combine(builder.Environment.WebRootPath, "images");
        builder.Services.AddScoped(_ => new ImageHandler(localFilePath));
        */

        builder.Services.AddScoped(typeof(ICacheHandler<>), typeof(CacheHandler<>));

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

        builder.Services.AddTransient<JwtTokenHandler>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        }); // Allows email to be used as username

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!);
            var issuer = builder.Configuration["Jwt:Issuer"]!;
            var audience = builder.Configuration["Jwt:Audience"]!;

            x.RequireHttpsMetadata = false; // Change on deploy
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
                ValidateAudience = true,
                ValidAudience = audience
            };
        });

        builder.Services.AddCors(x =>
        {
            x.AddPolicy("Strict", x =>
            {
                x.WithOrigins("https://happy-flower-079ea2e03.6.azurestaticapps.net")
                 .WithMethods("GET", "POST", "PUT", "DELETE")
                 .WithHeaders("Content-Type", "Authorization", "X-ADM-API-KEY")
                 .AllowCredentials();
            });

            x.AddPolicy("AllowAll", x =>
            {
                x.AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader();

            });
        });

        var app = builder.Build();
        /*
        await SeedData.SetRolesAsync(app);
        await SeedData.SetStatusAsync(app);
        */
        app.MapOpenApi();

        app.UseHttpsRedirection();

        //app.MapStaticAssets();

        app.UseCors("Strict");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/swagger/v1/swagger.json", "Alpha Portal API");
            o.RoutePrefix = string.Empty;
        });

        //app.MapControllers().WithStaticAssets();

        app.MapControllers();

        app.Run();
    }
}
