using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public interface ITagFriendsViewModel : IViewModel
    {
        ICollection<User> UserFriends { get; set; }
        ICollection<User> Tagged { get; set; }
    }
}
