namespace SocialMedia.Services.Image.ImageProcessing
{
    using Microsoft.Extensions.DependencyInjection;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Processing;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImageProcessingService : IImageProcessingService
    {
        private const int THUMBNAIL_WIDTH = 300;
        private const int FULLSCREEN_WIDTH = 1000;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ImageProcessingService(IServiceScopeFactory serviceScopeFactory)
        => this._serviceScopeFactory = serviceScopeFactory;

        public async Task ProcessAsync(IEnumerable<ImageInputModel> images)
        {
            var tasks = images
                .Select(image => Task.Run(async () =>
                {
                    using var imageResult = await Image.LoadAsync(image.Content);

                    var original = await this.SaveImageAsync(imageResult, imageResult.Width);
                    var fullscreen = await this.SaveImageAsync(imageResult, FULLSCREEN_WIDTH);
                    var thumbnail = await this.SaveImageAsync(imageResult, THUMBNAIL_WIDTH);

                    var data = this._serviceScopeFactory
                       .CreateScope()
                       .ServiceProvider
                       .GetRequiredService<SocialMediaDbContext>();

                    data.ImagesData.Add(new ImageData
                    {
                        OriginalFileName = image.Name,
                        OriginalType = image.Type,
                        OriginalContent = original,
                        FullscreenContent = fullscreen,
                        ThumbnailContent = thumbnail,
                        UploaderId = image.UploaderId
                    });

                    await data.SaveChangesAsync();
                }));

            await Task.WhenAll(tasks);
        }

        private async Task<byte[]> SaveImageAsync(Image image, int resizeWidth)
        {
            var width = image.Width;
            var height = image.Height;

            if (width > resizeWidth)
            {
                height = (int)((double)resizeWidth / width * height);
                width = resizeWidth;
            }

            //Resize the image
            image.Mutate(i => i
                    .Resize(new Size(width, height)));

            //Remove any info from the image
            image.Metadata.ExifProfile = null;

            var memoryStream = new MemoryStream();

            await image.SaveAsJpegAsync(memoryStream, new JpegEncoder
            {
                Quality = 75
            });

            return memoryStream.ToArray();
        }
    }
}
