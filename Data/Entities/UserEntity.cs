using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public UserProfileEntity? Profile { get; set; }
    public UserAddressEntity? Address { get; set; }
}
