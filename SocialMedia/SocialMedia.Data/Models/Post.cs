namespace SocialMedia.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Post
    {
        public Post()
        {
            this.Comments = new HashSet<Comment>();
            this.TaggedUsers = new HashSet<TagFriendInPost>();
        }

        public int PostId { get; set; }
        public DateTime DatePosted { get; set; }
        public string Content { get; set; }
        public string AuthorId { get; set; }
        public User Author { get; set; }
        public int? GroupId { get; set; }
        public Group Group { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<TagFriendInPost> TaggedUsers { get; set; }
    }
}
