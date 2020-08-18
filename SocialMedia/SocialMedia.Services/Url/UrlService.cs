namespace SocialMedia.Services.Url
{
    using Microsoft.AspNetCore.Http;

    public class UrlService : IUrlService
    {
        public string GenerateReturnUrl(string path, HttpContext httpContext)
        {
            var host = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            return $"{host}/{path}";
        }
    }
}
