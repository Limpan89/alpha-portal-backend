using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<UserEntity>(options)
{
    public DbSet<UserProfileEntity> UserProfiles { get; set; }
    public DbSet<UserAddressEntity> UserAddresses { get; set; }
    public DbSet<ProjectEntity> Projects { get; set; }
    public DbSet<StatusEntity> Status { get; set; }
    public DbSet<ClientEntity> Clients { get; set; }
}
