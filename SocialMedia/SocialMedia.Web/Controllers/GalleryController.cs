namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Services.Image;
    using SocialMedia.Services.Stream;
    using SocialMedia.Services.User;
    using SocialMedia.Web.Models;
    using System.Threading.Tasks;

    public class GalleryController : Controller
    {
        //private readonly IImageService _imageService;
        //private readonly IStreamService _streamService;
        //private readonly IUserService _userService;

        //public GalleryController(
        //    IImageService imageService,
        //    IStreamService streamService,
        //    IUserService userService)
        //{
        //    this._imageService = imageService;
        //    this._streamService = streamService;
        //    this._userService = userService;
        //}

        //public async Task<IActionResult> IndexAsync(string userId)
        //{
        //    var user = await this._userService
        //        .GetUserByIdAsync(userId);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    var images = this._imageService
        //        .GetAllImagesByUserId(userId);

        //    return View(images);
        //}

        //[HttpGet]
        //public IActionResult AddImage()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddImage(IFormCollection uploadedFiles)
        //{
        //    var userId = await this._userService
        //        .GetUserIdByNameAsync(User.Identity.Name);

        //    var images = uploadedFiles.Files;

        //    if (images == null)
        //    {
        //        return RedirectToAction(
        //            nameof(IndexAsync),
        //            new { userId = userId });
        //    }

        //    foreach (var image in images)
        //    {
        //        var memoryStream = await this._streamService
        //            .CopyFileToMemoryStreamAsync(image);

        //        await this._imageService.AddImageAsync(new ImageServiceModel()
        //        {
        //            ImageTitle = image.FileName,
        //            ImageData = memoryStream.ToArray(),
        //            UploaderId = userId
        //        });
        //    }

        //    return RedirectToAction(
        //        "Index",
        //        new { userId = userId });
        //}

        //[HttpGet]
        //public async Task<IActionResult> DeleteImageAsync(int imageId)
        //{
        //    if (!await this._imageService.IsImageExistAsync(imageId))
        //    {
        //        return NotFound($"Image cannot be found!");
        //    }
        //    return View(new ImageViewModel
        //    {
        //        ImageId = imageId,
        //        Base64Image = await this._imageService
        //            .GetImageByIdAsync(imageId)
        //    });
        //}

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int imageId)
        //{
        //    var userId = await this._userService
        //        .GetUserIdByNameAsync(User.Identity.Name);

        //    await this._imageService.DeleteImageAsync(imageId);

        //    return RedirectToAction(
        //        "Index",
        //        new { userId = userId });
        //}
    }
}