using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public class CommentTagFriendsViewModel : ITagFriendsViewModel
    {
        public CommentTagFriendsViewModel()
        {
            this.UserFriends = new List<User>();
            this.Tagged = new List<User>();
        }

        public ICollection<User> UserFriends { get; set; }
        public ICollection<User> Tagged { get; set; }
        public Comment Comment { get; set; }
        public User CurrentUser { get; set; }
    }
}
