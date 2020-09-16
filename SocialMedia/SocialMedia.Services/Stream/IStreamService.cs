namespace SocialMedia.Services.Stream
{
    using Microsoft.AspNetCore.Http;
    using SocialMedia.Services.Common;
    using System.IO;
    using System.Threading.Tasks;

    public interface IStreamService : IService
    {
        Task<MemoryStream> CopyFileToMemoryStreamAsync(IFormFile file);
    }
}
