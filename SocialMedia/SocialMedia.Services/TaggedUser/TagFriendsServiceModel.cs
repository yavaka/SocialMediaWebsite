namespace SocialMedia.Services.TaggedUser
{
    using SocialMedia.Services.User;
    using System.Collections.Generic;

    public class TagFriendsServiceModel
    {
        public IList<UserServiceModel> Friends { get; set; }

        public IList<UserServiceModel> TaggedFriends { get; set; }
    }
}
