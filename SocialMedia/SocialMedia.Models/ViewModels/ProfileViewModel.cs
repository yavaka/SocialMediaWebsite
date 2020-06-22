using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public class ProfileViewModel : IViewModel
    {
        public User CurrentUser { get; set; }

        public ICollection<Post> PostsComments { get; set; }

        public ICollection<User> Friends { get; set; }
    }
}
