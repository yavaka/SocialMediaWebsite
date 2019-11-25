using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models
{
    public class Comment
    {
        public Comment()
        {
            //this.TaggedFriends = new List<User>();
        }

        public int CommentId{ get; set; }
        public User Author { get; set; }
        public int AuthorId => this.Author.UserId;
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }

        //public virtual ICollection<User> TaggedFriends { get; set; }
    }
}
