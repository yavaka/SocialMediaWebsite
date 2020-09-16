namespace SocialMedia.Services.Image
{
    public class AvatarServiceModel 
    {
        public int AvatarId { get; set; }
        public byte[] AvatarData { get; set; }
        public string UploaderId { get; set; }
    }
}
