namespace SocialMedia.Data.Models
{
    public class Avatar
    {
        public int Id { get; set; }
        public byte[] AvatarData { get; set; }
        public string UploaderId { get; set; }
        public User Uploader { get; set; }
    }
}
