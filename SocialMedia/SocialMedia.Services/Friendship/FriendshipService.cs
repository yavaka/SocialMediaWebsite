namespace SocialMedia.Services.Friendship
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.User;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FriendshipService : IFriendshipService
    {
        private readonly SocialMediaDbContext _data;

        public FriendshipService(SocialMediaDbContext data) => this._data = data;

        public async Task<IList<UserServiceModel>> GetFriendsAsync(string userId)
        {
            //Get current user friendships where it is addressee or requester
            var friendships = await GetFriendshipsByUserIdAsync(userId);

            var addressee = friendships
                .Select(a => a.Addressee)
                .ToList();

            var requesters = friendships
                .Select(r => r.Requester)
                .ToList();

            var friends = addressee
                .Concat(requesters)
                .ToList();

            friends.RemoveAll(u => u.Id == userId);

            return friends;
        }

        public async Task<ICollection<UserServiceModel>> GetNonFriendsAsync(string userId)
        {
            var nonFriends = await this._data.Users
                .Where(i => i.Id != userId)
                .Select(u => new UserServiceModel
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    FullName = u.FullName,
                    Country = u.Country,
                    DateOfBirth = u.DOB
                })
                .ToListAsync();

            var friends = await GetFriendsAsync(userId);
            var pendingRequests = await GetPendingRequestsAsync(userId);
            var friendRequests = await GetFriendRequestsAsync(userId);

            nonFriends.RemoveAll(u =>
                friends.Any(f => f.Id == u.Id));
            nonFriends.RemoveAll(u =>
                pendingRequests.Any(f => f.Id == u.Id));
            nonFriends.RemoveAll(u =>
                friendRequests.Any(f => f.Id == u.Id));

            return nonFriends;
        }

        private async Task<IEnumerable<FriendshipServiceModel>> GetFriendshipsByUserIdAsync(string userId)
           => await this._data.Friendships
                .Where(i => i.AddresseeId == userId && i.Status == Status.Accepted ||
                            i.RequesterId == userId && i.Status == Status.Accepted)
                .Select(f => new FriendshipServiceModel
                {
                    Addressee = new UserServiceModel 
                    {
                        Id = f.Addressee.Id,
                        UserName = f.Addressee.UserName,
                        FullName = f.Addressee.FullName,
                        Country = f.Addressee.Country,
                        DateOfBirth = f.Addressee.DOB
                    },
                    Requester = new UserServiceModel 
                    {
                        Id = f.Requester.Id,
                        UserName = f.Requester.UserName,
                        FullName = f.Requester.FullName,
                        Country = f.Requester.Country,
                        DateOfBirth = f.Requester.DOB
                    }
                })
                .ToListAsync();

        public async Task<ServiceModelFriendshipStatus> GetFriendshipStatusAsync(string currentUserId, string userId)
        {
            var friendship = await this._data
                .Friendships
                    .FirstOrDefaultAsync(u =>
                        u.AddresseeId == currentUserId && u.RequesterId == userId ||
                        u.AddresseeId == userId && u.RequesterId == currentUserId);

            if (friendship == null)
            {
                return ServiceModelFriendshipStatus.NonFriends;
            }

            switch (friendship.Status)
            {
                case Status.Accepted:
                    return ServiceModelFriendshipStatus.Accepted;
                case Status.Pending:
                    return ServiceModelFriendshipStatus.Pending;
            }
            return ServiceModelFriendshipStatus.Request;
        }

        public async Task<IEnumerable<UserServiceModel>> GetFriendRequestsAsync(string currentUserId)
        => await this._data
            .Friendships
                .Where(a => a.AddresseeId == currentUserId && a.Status == Status.Pending)
                .Select(r => new UserServiceModel 
                {
                        Id = r.Requester.Id,
                        UserName = r.Requester.UserName,
                        FullName = r.Requester.FullName,
                        Country = r.Requester.Country,
                        DateOfBirth = r.Requester.DOB
                })
                .ToListAsync();

        public async Task<IEnumerable<UserServiceModel>> GetPendingRequestsAsync(string currentUserId)
        => await this._data.Friendships
                .Where(r => r.RequesterId == currentUserId && r.Status == Status.Pending)
                .Select(a => new UserServiceModel 
                {
                    Id = a.Addressee.Id,
                    UserName = a.Addressee.UserName,
                    FullName = a.Addressee.FullName,
                    Country = a.Addressee.Country,
                    DateOfBirth = a.Addressee.DOB
                })
                .ToListAsync();

        public async Task<IEnumerable<UserServiceModel>> GetFriendsByPartNameAsync(string partName, string userId)
        {
            var userFriends = await GetFriendsAsync(userId);

            return userFriends
                .Where(f => f.FullName.ToLower().StartsWith(partName.ToLower()))
                .ToList();
        }

        public async Task SendRequestAsync(string currentUserId, string addresseeId)
        {
            if (!await IsFriendshipExistAsync(currentUserId, addresseeId))
            {
                var friendship = new Friendship()
                {
                    AddresseeId = addresseeId,
                    RequesterId = currentUserId,
                    Status = Status.Pending
                };

                await this._data.Friendships.AddAsync(friendship);
                await this._data.SaveChangesAsync();
            }
        }

        private async Task<bool> IsFriendshipExistAsync(string currentUserId, string addresseeId)
        => await this._data
            .Friendships
                .AnyAsync(u => u.RequesterId == currentUserId &&
                            u.AddresseeId == addresseeId ||
                            u.RequesterId == addresseeId &&
                            u.AddresseeId == currentUserId);

        public async Task AcceptRequestAsync(string currentUserId, string requesterId)
        {
            if (await IsFriendshipExistAsync(currentUserId, requesterId))
            {
                var friendship = await GetFriendshipAsync(requesterId, currentUserId);

                if (friendship.Status == Status.Pending)
                {
                    friendship.Status = Status.Accepted;

                    await this._data.SaveChangesAsync();
                }
            }
        }

        public async Task RejectRequestAsync(string currentUserId, string requesterId)
        {
            if (await IsFriendshipExistAsync(currentUserId, requesterId))
            {
                var friendship = await GetFriendshipAsync(requesterId, currentUserId);

                await RemoveFriendshipAsync(friendship);
            }
        }

        public async Task CancelInvitationAsync(string currentUserId, string addresseeId)
        {
            if (await IsFriendshipExistAsync(currentUserId, addresseeId))
            {
                var friendship = await GetFriendshipAsync(currentUserId, addresseeId);

                await RemoveFriendshipAsync(friendship);
            }
        }

        public async Task UnfriendAsync(string currentUserId, string friendId)
        {
            if (await IsFriendshipExistAsync(currentUserId, friendId))
            {
                var friendship = await GetFriendshipAsync(currentUserId, friendId) == null ?
                    await GetFriendshipAsync(friendId, currentUserId) :
                    await GetFriendshipAsync(currentUserId, friendId);

                await RemoveFriendshipAsync(friendship);
            }
        }

        private async Task<Friendship> GetFriendshipAsync(string requesterId, string addresseeId)
        => await this._data
            .Friendships
                .FirstOrDefaultAsync(i => i.RequesterId == requesterId &&
                                        i.AddresseeId == addresseeId);

        private async Task RemoveFriendshipAsync(Friendship friendship)
        {
            if (friendship != null)
            {
                this._data.Friendships.Remove(friendship);

                await this._data.SaveChangesAsync();
            }
        }
    }
}
