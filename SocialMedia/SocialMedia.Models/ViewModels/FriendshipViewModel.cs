using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Models.ViewModels
{
    public class FriendshipViewModel : IViewModel
    {
        public FriendshipViewModel()
        {
            this.Requests = new HashSet<User>();
            this.Pending = new HashSet<User>();
        }

        /// <summary>
        /// Users who sent request to the current user
        /// </summary>
        public ICollection<User> Requests { get; set; }
        
        /// <summary>
        /// Current user`s pending requests
        /// </summary>
        public ICollection<User> Pending { get; set; }

        public User CurrentUser { get; set; }
    }
}
