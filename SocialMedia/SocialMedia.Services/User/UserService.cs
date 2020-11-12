namespace SocialMedia.Services.User
{
    using Microsoft.AspNetCore.Identity;
    using SocialMedia.Data.Models;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager) => this._userManager = userManager;
      
        public async Task<UserServiceModel> GetUserByIdAsync(string userId)
        {
            var user = await this._userManager.FindByIdAsync(userId);
            return new UserServiceModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Country = user.Country,
                DateOfBirth = user.DOB
            };
        }

        public async Task<string> GetUserIdByNameAsync(string name)
        {
            var user = await this._userManager
                .FindByNameAsync(name);

            return user.Id;
        }

        public async Task<UserServiceModel> GetUserByNameAsync(string name)
        {
            var user = await this._userManager
                .FindByNameAsync(name);

            return new UserServiceModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Country = user.Country,
                DateOfBirth = user.DOB
            };
        }
    }
}
