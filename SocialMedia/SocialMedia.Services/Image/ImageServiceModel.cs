using System;

namespace SocialMedia.Services.Image
{
    public class ImageServiceModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public byte[] OriginalContent { get; set; }
        public DateTime UploadDate { get; set; }
        /// <summary>
        /// There are 4 groups
        /// </summary>
        public int GroupNum { get; set; }
    }
}
