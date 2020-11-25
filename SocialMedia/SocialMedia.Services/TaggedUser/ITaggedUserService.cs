namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Common;
    using SocialMedia.Services.User;

    public interface ITaggedUserService : IService
    {
        ICollection<TagFriendInPost> GetTagFriendsInPostsEntities(
            string taggerId,
            IEnumerable<string> taggedFriendsIds);

        ICollection<TagFriendInComment> GetTagFriendsInCommentsEntities(
            string taggerId,
            IEnumerable<string> taggedFriendsIds);

        Task TagFriendPost(string taggerId, string taggedId, int postId);

        Task TagFriendComment(string taggerId, string taggedId, int commentId);

        Task UpdateTaggedFriendsInPostAsync(IList<UserServiceModel> taggedFriends, int postId, string taggerId);

        Task UpdateTaggedFriendsInCommentAsync(IList<UserServiceModel> taggedFriends, int commentId, string taggerId);

        Task RemoveTaggedFriendPost(string taggedId, int postId);

        Task RemoveTaggedFriendComment(string taggedId, int commentId);

        Task DeleteTaggedFriendsPostId(int postId);

        Task DeleteTaggedFriendsCommentId(int commentId);

        Task DeleteTaggedFriendsInComments(ICollection<int> commentsIds);
    }
}
