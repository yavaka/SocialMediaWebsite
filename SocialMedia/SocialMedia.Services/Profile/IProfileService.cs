namespace SocialMedia.Services.Profile
{
    using SocialMedia.Services.Common;
    using System.Threading.Tasks;
    
    public interface IProfileService : IService
    {
        Task<ProfileServiceModel> GetProfileAsync(string userId);
    }
}
