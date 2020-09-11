namespace SocialMedia.Data.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ImageData { get; set; }
        public bool IsAvatar { get; set; }
        public string UploaderId { get; set; }
        public User Uploader{ get; set; }
    }
}
