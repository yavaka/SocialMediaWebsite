using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialMedia.Models
{
    public class Comment
    {
        public Comment()
        {
            this.TaggedUsers = new HashSet<TagFriends>();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }
        public string AuthorId { get; set; }
        public virtual User Author { get; set; }
        public int CommentedPostId { get; set; }
        public virtual Post CommentedPost { get; set; }

        public virtual ICollection<TagFriends> TaggedUsers { get; set; }

        [NotMapped]
        public string Message { get; set; }
    }
}
