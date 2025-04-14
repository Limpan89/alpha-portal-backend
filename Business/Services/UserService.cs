using Business.Factories;
using Business.Models;
using Data.Entities;
using Data.Repositories;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Services
{
    public interface IUserService
    {
        Task<ServiceResult> AddUserAsync(AddUserFormDto form);
        Task<ServiceResult> DeleteUserAsync(string userId);
        Task<ServiceResult<IEnumerable<UserModel>>> GetAllUsersAsync();
        Task<ServiceResult<UserModel>> GetUserByEmailAsync(string email);
        Task<ServiceResult<UserModel>> GetUserByIdAsync(string userId);
        Task<ServiceResult> UpdateUserAsync(EditUserFormDto form);
    }

    public class UserService(IUserRepository userRepo, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManger, IPostalAddressService postalService) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;
        private readonly UserManager<UserEntity> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManger = roleManger;
        private readonly IPostalAddressService _postalService = postalService;
        private readonly IUserEntityFactory _userFactory = new UserEntityFactory();

        public async Task<ServiceResult<UserModel>> GetUserByIdAsync(string userId)
        {
            var result = await _userRepo.GetAsync(u => u.Id == userId, i => i.Address!.PostalAddress);
            return result.MapTo<ServiceResult<UserModel>>();
        }

        public async Task<ServiceResult<UserModel>> GetUserByEmailAsync(string email)
        {
            var result = await _userRepo.GetAsync(u => u.Email == email, i => i.Address!.PostalAddress);
            return result.MapTo<ServiceResult<UserModel>>();
        }

        public async Task<ServiceResult<IEnumerable<UserModel>>> GetAllUsersAsync()
        {
            var result = await _userRepo.GetAllAsync(orderByDesc: false, sortBy: u => u.Profile!.LastName, filterBy: null, i => i.Address!.PostalAddress);
            return result.MapTo<ServiceResult<IEnumerable<UserModel>>>();
        }

        public async Task<ServiceResult> AddUserAsync(AddUserFormDto form)
        {
            if (form == null)
                return new ServiceResult { Succeeded = false, StatusCode = 400 };

            var entity = _userFactory.MapModelToEntity(form);

            var any = await _postalService.AnyPostalAddressAsync(form.PostalCode);
            if (!any.Succeeded)
                entity.Address.PostalAddress = new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName };

            var result = await _userRepo.AddAsync(entity);
            return result.MapTo<ServiceResult>();
        }

        public async Task<ServiceResult> UpdateUserAsync(EditUserFormDto form)
        {
            if (form == null)
                return new ServiceResult { Succeeded = false, StatusCode = 400 };

            var entity = _userFactory.MapModelToEntity(form);

            var any = await _postalService.AnyPostalAddressAsync(form.PostalCode);
            if (!any.Succeeded)
                entity.Address.PostalAddress = new PostalAddressEntity { PostalCode = form.PostalCode, CityName = form.CityName };

            var result = await _userRepo.UpdateAsync(entity);
            return result.MapTo<ServiceResult>();
        }

        public async Task<ServiceResult> DeleteUserAsync(string userId)
        {
            var result = await _userRepo.DeleteAsync(u => u.Id == userId);
            return result.MapTo<ServiceResult>();
        }
    }
}
