namespace SocialMedia.Services.Image.ImageFetching
{
    using SocialMedia.Services.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.IO;

    public interface IImageFetchingService : IService
    {
        Task<List<string>> GetAllImagesByUserId(string userId);

        Task<Stream> GetThumbnail(string id);

        Task<Stream> GetFullscreen(string id);
    }
}
