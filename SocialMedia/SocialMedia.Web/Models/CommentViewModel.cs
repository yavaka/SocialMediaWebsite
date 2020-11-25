namespace SocialMedia.Web.Models
{
    using SocialMedia.Services.User;
    using System;

    public class CommentViewModel
    {
        public int CommentId { get; set; }

        public string Content { get; set; }

        public DateTime DatePosted { get; set; }

        public UserServiceModel Author { get; set; }

        /// <summary>
        /// Tagged Friends in a JSON format
        /// </summary>
        public string TaggedFriends { get; set; }

        public int PostId { get; set; }
    }
}
