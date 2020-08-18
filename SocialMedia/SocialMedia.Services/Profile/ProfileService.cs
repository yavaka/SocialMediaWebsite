namespace SocialMedia.Services.Profile
{
    using SocialMedia.Services.Post;
    using SocialMedia.Services.User;
    using System.Threading.Tasks;

    public class ProfileService : IProfileService
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public ProfileService(
            IPostService postService,
            IUserService userService)
        {
            this._postService = postService;
            this._userService = userService;
        }

        public async Task<ProfileServiceModel> GetProfileAsync(string userId)
            => new ProfileServiceModel()
            {
                User = await this._userService
                    .GetUserByIdAsync(userId),
                Posts = await this._postService
                    .GetPostsByUserIdAsync(userId)
            };
    }
}
