using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public class ProfileViewModel : IViewModel
    {
        public ProfileViewModel()
        {
            this.Posts = new List<PostTagFriendsViewModel>();
        }


        public User CurrentUser { get; set; }
        
        public ICollection<PostTagFriendsViewModel> Posts { get; set; }
    }
}
