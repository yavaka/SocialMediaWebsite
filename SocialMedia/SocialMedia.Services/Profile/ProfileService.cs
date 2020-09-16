namespace SocialMedia.Services.Profile
{
    using SocialMedia.Services.Image;
    using SocialMedia.Services.Post;
    using SocialMedia.Services.User;
    using System.Threading.Tasks;

    public class ProfileService : IProfileService
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public ProfileService(
            IPostService postService,
            IUserService userService,
            IImageService imageService)
        {
            this._postService = postService;
            this._userService = userService;
            this._imageService = imageService;
        }

        public async Task<ProfileServiceModel> GetProfileAsync(string userId)
            => new ProfileServiceModel()
            {
                User = await this._userService
                    .GetUserByIdAsync(userId),
                Posts = await this._postService
                    .GetPostsByUserIdAsync(userId),
                AvatarUrl =await this._imageService
                    .GetAvatarAsync(userId)
            };
    }
}
