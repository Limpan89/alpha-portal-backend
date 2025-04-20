using Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Util;

public static class SeedData
{
    public static async Task SetRolesAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = { "Admin", "User" };

        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
                await roleManager.CreateAsync(new IdentityRole(r));
        }
    }

    public static async Task SetStatusAsync(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var statusRepo = scope.ServiceProvider.GetRequiredService<IStatusRepository>();
        string[] statusNames = { "Started", "Completed" };

        foreach (var s in statusNames)
        {
            if (!(await statusRepo.ExistsAsync(e => e.StatusName == s)).Succeeded)
                await statusRepo.AddAsync(new Entities.StatusEntity { StatusName = s });
        }
    }
}