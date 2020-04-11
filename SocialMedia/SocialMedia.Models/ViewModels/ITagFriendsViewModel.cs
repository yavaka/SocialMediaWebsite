using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public interface ITagFriendsViewModel
    {
        ICollection<User> UserFriends { get; set; }
        ICollection<User> Tagged { get; set; }
        User CurrentUser { get; set; }
    }
}
