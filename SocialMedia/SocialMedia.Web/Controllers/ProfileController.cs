namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using SocialMedia.Services.Profile;
    using SocialMedia.Services.User;
    using SocialMedia.Services.Friendship;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;

        public ProfileController(
            IProfileService profileService,
            IUserService userService)
        {
            this._profileService = profileService;
            this._userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(
            string userId, 
            ServiceModelFriendshipStatus friendshipStatus)
        {
            var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            ProfileServiceModel profile;

            if (userId != null)
            {
                profile = await this._profileService.GetProfileAsync(userId);
            }
            else //Gets the current user`s profile
            {
                profile = await this._profileService.GetProfileAsync(currentUserId);
            }
            
            profile.FriendshipStatus = friendshipStatus;
            
            profile.CurrentUserId = currentUserId;

            return View(profile);
        }
    }
}