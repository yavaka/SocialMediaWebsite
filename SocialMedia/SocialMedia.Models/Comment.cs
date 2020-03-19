using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMedia.Models
{
    public class Comment
    {
        //public Comment()
        //{
        //    this.TaggedFriends = new HashSet<User>();
        //}

        [Key]
        public int Id{ get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public string AuthorId { get; set; }
        public virtual User Author { get; set; }
        public int CommentedPostId { get; set; }
        public virtual Post CommentedPost { get; set; }
        
        //TODO: Comment/TagFriends
        //public virtual ICollection<User> TaggedFriends { get; set; }
    }
}
