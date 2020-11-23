namespace SocialMedia.Services.Friendship
{
    using SocialMedia.Services.Common;
    using SocialMedia.Services.User;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFriendshipService : IService
    {
        Task<IList<UserServiceModel>> GetFriendsAsync(string userId);

        Task<ICollection<UserServiceModel>> GetNonFriendsAsync(string userId);

        Task<ServiceModelFriendshipStatus> GetFriendshipStatusAsync(string currentUserId, string secondUserId);

        Task<IEnumerable<UserServiceModel>> GetFriendRequestsAsync(string currentUserId);

        Task<IEnumerable<UserServiceModel>> GetPendingRequestsAsync(string currentUserId);

        Task<IEnumerable<UserServiceModel>> GetFriendsByPartNameAsync(string partName, string userId);

        Task SendRequestAsync(string currentUserId, string addresseeId);

        Task AcceptRequestAsync(string currentUserId, string requesterId);

        Task RejectRequestAsync(string currentUserId, string requesterId);

        Task CancelInvitationAsync(string currentUserId, string addresseeId);

        Task UnfriendAsync(string currentUserId, string friendId);
    }
}
