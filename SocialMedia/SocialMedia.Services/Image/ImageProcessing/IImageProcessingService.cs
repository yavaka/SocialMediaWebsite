namespace SocialMedia.Services.Image.ImageProcessing
{
    using SocialMedia.Services.Common;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IImageProcessingService : IService
    {
        Task ProcessAsync(IEnumerable<ImageInputModel> images);
    }
}
