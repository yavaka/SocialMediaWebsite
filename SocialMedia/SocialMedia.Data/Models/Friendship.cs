namespace SocialMedia.Data.Models
{
    public class Friendship
    {
        public string RequesterId { get; set; }

        public string AddresseeId { get; set; }

        public Status Status { get; set; }

        public User Addressee { get; set; }

        public User Requester { get; set; }
    }

    public enum Status
    {
        Pending = 0,
        Accepted = 1
    }
}
