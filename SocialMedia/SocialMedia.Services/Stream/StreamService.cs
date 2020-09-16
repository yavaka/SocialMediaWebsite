namespace SocialMedia.Services.Stream
{
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Threading.Tasks;

    public class StreamService : IStreamService
    {
        public async Task<MemoryStream> CopyFileToMemoryStreamAsync(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                return ms;
            }
        }
    }
}
