using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SocialMedia.Services.Stream
{
    public class StreamService : IStreamService
    {
        public async Task<MemoryStream> CopyFileToMemroyStreamAsync(IFormFile file)
        {
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                return ms;
            }
        }
    }
}
