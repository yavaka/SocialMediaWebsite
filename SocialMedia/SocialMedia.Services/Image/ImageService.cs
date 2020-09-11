namespace SocialMedia.Services.Image
{
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class ImageService : IImageService
    {
        private readonly SocialMediaDbContext _data;

        public ImageService(SocialMediaDbContext context) => this._data = context;
        
        public async Task AddImage(ImageServiceModel serviceModel)
        {
            var img = new Image
            {
                ImageTitle = serviceModel.ImageTitle,
                ImageData = serviceModel.ImageData,
                UploaderId = serviceModel.UploaderId,
                IsAvatar = serviceModel.IsAvatar
            };

            this._data.Images.Add(img);
            await this._data.SaveChangesAsync();
        }

        public async Task DeleteAvatar(string userId)
        {
            if (IsThereAvatar(userId))
            {
                var avatar = GetImages(userId)
                    .FirstOrDefault(a => a.IsAvatar == true);

                this._data.Images.Remove(avatar);
                await this._data.SaveChangesAsync();
            }
        }

        public bool IsThereAvatar(string userId)
        => GetImages(userId).Any(a => a.IsAvatar == true);

        public string GetAvatar(string userId)
        {
            if (IsThereAvatar(userId))
            {
                var avatar = GetImages(userId)
                    .FirstOrDefault(a => a.IsAvatar == true);
                
                return GetImageDataUrl(avatar.ImageData);
            }
            return null;
        }

        private ICollection<Image> GetImages(string userId)
         => this._data.Images
            .Where(i => i.UploaderId == userId)
            .ToList();

        private string GetImageDataUrl(byte[] imageData)
        {
            var imageBase64 = Convert.ToBase64String(imageData);

            return string.Format(
                    "data:image/jpg;base64,{0}",
                    imageBase64);
        }
    }
}
