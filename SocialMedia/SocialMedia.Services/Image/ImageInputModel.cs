namespace SocialMedia.Services.Image
{
    using System;
    using System.IO;

    public class ImageInputModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Stream Content { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploaderId { get; set; }
    }
}
