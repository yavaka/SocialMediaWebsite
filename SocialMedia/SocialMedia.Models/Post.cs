using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models
{
    public class Post
    {
        public Post() 
        {
            this.Comments = new HashSet<Comment>();
            //this.TaggedFriend = new HashSet<User>();
        }


        public int PostId { get; set; }
        public DateTime DatePosted { get; set; }
        public string Content { get; set; }
        public virtual User Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        //public virtual ICollection<User> TaggedFriend { get; set; }

    }
}
