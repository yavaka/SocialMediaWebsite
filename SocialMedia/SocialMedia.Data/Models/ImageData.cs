namespace SocialMedia.Data.Models
{
    using System;

    public class ImageData
    {
        public ImageData() => this.Id = Guid.NewGuid();

        public Guid Id { get; set; }
        public string OriginalFileName { get; set; }
        public string OriginalType { get; set; }
        public byte[] OriginalContent { get; set; }
        public byte[] FullscreenContent { get; set; }
        public byte[] ThumbnailContent { get; set; }
        public string UploaderId { get; set; }
        public User Uploader { get; set; }
    }
}
