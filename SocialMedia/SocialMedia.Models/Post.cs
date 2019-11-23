using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models
{
    public class Post
    {
        public Post()
        {
            this.TaggedFriend = new List<User>();
            this.Comments = new List<Comment>();
        }


        public int PostId { get; set; }
        public User Author { get; set; }
        public int AuthorId => this.Author.UserId;
        public DateTime DatePosted { get; set; }
        public string Content { get; set; }
        public List<User> TaggedFriend { get; set; }
        public List<Comment> Comments { get; set; }

    }
}
