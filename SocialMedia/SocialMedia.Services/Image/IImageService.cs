namespace SocialMedia.Services.Image
{
    using SocialMedia.Services.Common;
    using System.Threading.Tasks;
    
    public interface IImageService : IService
    {
        Task AddImage(ImageServiceModel serviceModel);
        Task DeleteAvatar(string userId);
        string GetAvatar(string userId);
        bool IsThereAvatar(string userId);
    }
}
