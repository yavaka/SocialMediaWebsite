using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models
{
    public class TagFriends
    {
        /// <summary>
        /// User who tag a friend
        /// </summary>
        public string TaggerId { get; set; }
        public User Tagger { get; set; }

        /// <summary>
        /// User who is tagged by some of its friends
        /// </summary>
        public string TaggedId { get; set; }
        public User Tagged { get; set; }

        public int? PostId { get; set; }
        public Post Post { get; set; }

        public int? CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}
