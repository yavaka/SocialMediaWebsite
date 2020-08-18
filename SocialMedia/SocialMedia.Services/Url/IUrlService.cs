namespace SocialMedia.Services.Url
{
    using Microsoft.AspNetCore.Http;
    using SocialMedia.Services.Common;

    public interface IUrlService : IService
    {
        string GenerateReturnUrl(string path, HttpContext httpContext);
    }
}
