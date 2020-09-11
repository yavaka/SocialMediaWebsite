namespace SocialMedia.Services.Image
{
    public class ImageServiceModel
    {
        public int ImageId { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ImageData { get; set; }
        public bool IsAvatar { get; set; }
        public string UploaderId { get; set; }
    }
}
