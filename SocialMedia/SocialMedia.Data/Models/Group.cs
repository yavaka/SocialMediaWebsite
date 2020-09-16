namespace SocialMedia.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Group
    {
        public Group()
        {
            this.Members = new HashSet<UserInGroup>();
            this.Posts = new HashSet<Post>();
        }

        public int GroupId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public ICollection<UserInGroup> Members { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
