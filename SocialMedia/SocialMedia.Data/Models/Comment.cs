namespace SocialMedia.Data.Models
{
    using System;
    using System.Collections.Generic;

    public class Comment
    {
        public Comment() => this.TaggedUsers = new HashSet<TagFriendInComment>();

        public int Id { get; set; }

        public string Content { get; set; }

        public DateTime DatePosted { get; set; }

        public string AuthorId { get; set; }

        public User Author { get; set; }

        public int CommentedPostId { get; set; }

        public Post CommentedPost { get; set; }

        public ICollection<TagFriendInComment> TaggedUsers { get; set; }
    }
}
