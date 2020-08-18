namespace SocialMedia.Services.User
{
    using System.Threading.Tasks;
    using System.Security.Claims;
    using SocialMedia.Services.Common;

    public interface IUserService : IService
    {
        Task<UserServiceModel> GetUserByIdAsync(string userId);

        Task<UserServiceModel> GetCurrentUserAsync(ClaimsPrincipal principal);

        string GetUserId(ClaimsPrincipal principal);
    }
}
