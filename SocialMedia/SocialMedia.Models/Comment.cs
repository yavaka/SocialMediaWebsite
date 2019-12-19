using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models
{
    public class Comment
    {
        //public Comment()
        //{
        //    this.TaggedFriends = new HashSet<User>();
        //}

        public int CommentId{ get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }

        //public int AuthorId { get; set; }
        //public virtual User Author { get; set; }
        //public virtual ICollection<User> TaggedFriends { get; set; }
    }
}
