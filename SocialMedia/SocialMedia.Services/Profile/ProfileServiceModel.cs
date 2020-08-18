namespace SocialMedia.Services.Profile
{
    using SocialMedia.Services.User;
    using SocialMedia.Services.Post;
    using System.Collections.Generic;
    using System.Linq;
    using SocialMedia.Services.Friendship;

    public class ProfileServiceModel
    {
        public ProfileServiceModel()
        {
            this._posts = new List<PostServiceModel>();
        }

        public UserServiceModel User { get; set; }

        public string CurrentUserId { get; set; }

        private List<PostServiceModel> _posts;
        public ICollection<PostServiceModel> Posts
        {
            get => this._posts;
            set
            {
                if (value.Count > 0)
                {
                    this._posts = value
                        .OrderByDescending(d => d.DatePosted)
                        .ToList();
                }
            }
        }

        /// <summary>
        /// Depending of the enum value it will be generated different layout
        /// </summary>
        public ServiceModelFRStatus FriendshipStatus{ get; set; }
    }
}
