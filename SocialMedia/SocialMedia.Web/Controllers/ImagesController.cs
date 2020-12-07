namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;
    using SocialMedia.Services.Image;
    using SocialMedia.Services.Image.ImageFetching;
    using SocialMedia.Services.Image.ImageProcessing;
    using SocialMedia.Services.User;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    public class ImagesController : Controller
    {
        /// <summary>
        /// 100 MB
        /// </summary>
        private const int MAX_REQUEST_SIZE = 100 * 1024 * 1024;
        /// <summary>
        /// 10 MB
        /// </summary>
        private const int MAX_IMAGE_SIZE = 10 * 1024 * 1024;
        private const int MAX_UPLOADED_IMAGES = 10;

        private readonly IUserService _userService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly IImageFetchingService _imageFetchingService;

        public ImagesController(
            IUserService userService,
            IImageProcessingService imageProcessingService,
            IImageFetchingService imageFetchingService)
        {
            this._userService = userService;
            this._imageProcessingService = imageProcessingService;
            this._imageFetchingService = imageFetchingService;
        }

        public IActionResult Upload() => View();

        [HttpPost]
        [RequestSizeLimit(MAX_REQUEST_SIZE)]
        public async Task<IActionResult> Upload(IFormFile[] images)
        {
            if (images.Length == 0)
            {
                this.ModelState.AddModelError("images", "There is no uploaded images! Please upload at least 1 image.");
                return View();
            }
            if (images.Length > MAX_UPLOADED_IMAGES)
            {
                this.ModelState.AddModelError("images", "Cannot be uploaded more than 10 images!");
                return View();
            }
            if (images.Any(i => i.Length > MAX_IMAGE_SIZE))
            {
                this.ModelState.AddModelError("images", "Image cannot be more than 10MB!");
                return View();
            }

            var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            await this._imageProcessingService.ProcessAsync(images.Select(i => new ImageInputModel
            {
                Name = i.FileName,
                Type = i.ContentType,
                Content = i.OpenReadStream(),
                UploaderId = currentUserId
            }));

            return RedirectToAction(nameof(GalleryAsync));
        }

        public async Task<IActionResult> GalleryAsync()
        {
            var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            var imagesIds = await this._imageFetchingService
                .GetAllImagesByUserId(currentUserId);

            return View(imagesIds);
        }

        public async Task<IActionResult> Thumbnail(string id)
           => this.ReturnImage(await this._imageFetchingService.GetThumbnail(id));

        public async Task<IActionResult> Fullscreen(string id)
            => this.ReturnImage(await this._imageFetchingService.GetFullscreen(id));

        private IActionResult ReturnImage(Stream image)
        {
            var headers = this.Response.GetTypedHeaders();

            headers.CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromDays(30)
            };

            headers.Expires = new DateTimeOffset(DateTime.UtcNow.AddDays(30));

            return this.File(image, "image/jpeg");
        }
    }
}
