using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class AppUserEntity : IdentityUser
{
    public AppUserProfileEntity? Profile { get; set; }
    public AppUserAddressEntity? Address { get; set; }
}