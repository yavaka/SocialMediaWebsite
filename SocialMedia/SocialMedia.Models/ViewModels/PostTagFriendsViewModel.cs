using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public class PostTagFriendsViewModel : ITagFriendsViewModel
    {
        public PostTagFriendsViewModel()
        {
            this.UserFriends = new List<User>();
            this.Tagged = new List<User>();
        }

        public Post Post { get; set; }
        public User CurrentUser { get; set; }
        public ICollection<User> UserFriends { get; set; }
        public ICollection<User> Tagged { get; set; }
    }
}
