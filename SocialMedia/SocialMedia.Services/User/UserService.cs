namespace SocialMedia.Services.User
{
    using Microsoft.AspNetCore.Identity;
    using SocialMedia.Data.Models;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager) => this._userManager = userManager;

        public async Task<UserServiceModel> GetCurrentUserAsync(ClaimsPrincipal principal)
        {
            var currentUser = await this._userManager.GetUserAsync(principal);
            return new UserServiceModel
            {
                Id = currentUser.Id,
                UserName = currentUser.UserName,
                FullName = currentUser.FullName,
                Country = currentUser.Country,
                DateOfBirth = currentUser.DOB
            };
        }
        
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
        
        public string GetUserId(ClaimsPrincipal principal)
        => this._userManager.GetUserId(principal);
    }
}
