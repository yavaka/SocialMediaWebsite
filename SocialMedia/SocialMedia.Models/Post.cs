using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string AuthorId { get; set; }
        public virtual User Author { get; set; }
        public int? GroupId { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<TagFriends> TaggedUsers { get; set; }

        //TODO: Posts/CheckIns
        [NotMapped]
        public string Message { get; set; }
    }
}
