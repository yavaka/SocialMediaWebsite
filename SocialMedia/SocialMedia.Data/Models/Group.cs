namespace SocialMedia.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual ICollection<UserInGroup> Members { get; set; }
        public virtual ICollection<Post> Posts { get; set; }


        [NotMapped]
        public string Message { get; set; }

    }
}
