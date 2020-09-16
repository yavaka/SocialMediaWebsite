namespace SocialMedia.Services.Friendship
{
    using SocialMedia.Services.User;
    using System.Collections.Generic;
    
    public class FriendshipServiceModel
    {
        public UserServiceModel Addressee { get; set; }

        public UserServiceModel Requester { get; set; }

        public ServiceModelFriendshipStatus Status{ get; set; }

        /// <summary>
        /// Users who sent request to the current user
        /// </summary>
        public IEnumerable<UserServiceModel> Requests { get; set; }

        /// <summary>
        /// Current user`s pending requests
        /// </summary>
        public IEnumerable<UserServiceModel> PendingRequests { get; set; }
    }
    /// <summary>
    /// Friendship status with current user
    /// </summary>
    public enum ServiceModelFriendshipStatus 
    {
        CurrentUser,
        NonFriends,
        Request,
        Pending,
        Accepted
    }
}
