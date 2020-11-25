namespace SocialMedia.Services.TaggedUser
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.User;

    public class TaggedUserService : ITaggedUserService
    {
        private readonly SocialMediaDbContext _data;

        public TaggedUserService(SocialMediaDbContext data) => this._data = data;

        public ICollection<TagFriendInPost> GetTagFriendsInPostsEntities(
            string taggerId,
            IEnumerable<string> taggedFriendsIds)
        {
            // Remove duplicate values
            var uniqueIds = new HashSet<string>(taggedFriendsIds);
            taggedFriendsIds = uniqueIds.ToList();

            var entities = new List<TagFriendInPost>();
            foreach (var taggedId in taggedFriendsIds)
            {
                entities.Add(new TagFriendInPost
                {
                    TaggerId = taggerId,
                    TaggedId = taggedId
                });
            }
            return entities;
        }

        public ICollection<TagFriendInComment> GetTagFriendsInCommentsEntities(
            string taggerId,
            IEnumerable<string> taggedFriendsIds)
        {
            // Remove duplicate values
            var uniqueIds = new HashSet<string>(taggedFriendsIds);
            taggedFriendsIds = uniqueIds.ToList();

            var entities = new List<TagFriendInComment>();
            foreach (var taggedId in taggedFriendsIds)
            {
                entities.Add(new TagFriendInComment
                {
                    TaggerId = taggerId,
                    TaggedId = taggedId
                });
            }
            return entities;
        }

        public async Task TagFriendPost(string taggerId, string taggedId, int postId)
        {
            await this._data.AddAsync(new TagFriendInPost
            {
                TaggerId = taggerId,
                TaggedId = taggedId,
                PostId = postId
            });

            await this._data.SaveChangesAsync();
        }

        public async Task TagFriendComment(string taggerId, string taggedId, int commentId)
        {
            await this._data.AddAsync(new TagFriendInComment
            {
                TaggerId = taggerId,
                TaggedId = taggedId,
                CommentId = commentId
            });

            await this._data.SaveChangesAsync();
        }

        public async Task RemoveTaggedFriendPost(string taggedId, int postId)
        {
            var entity = await this._data.TagFriendsInPosts
                .FirstOrDefaultAsync(u => u.TaggedId == taggedId &&
                                         u.PostId == postId);

            this._data.TagFriendsInPosts.Remove(entity);
            await this._data.SaveChangesAsync();
        }

        public async Task RemoveTaggedFriendComment(string taggedId, int commentId)
        {
            var entity = await this._data.TagFriendsInComments
                .FirstOrDefaultAsync(u => u.TaggedId == taggedId &&
                                        u.CommentId == commentId);
            this._data.TagFriendsInComments.Remove(entity);
            await this._data.SaveChangesAsync();
        }

        public async Task DeleteTaggedFriendsPostId(int postId)
        {
            var entities = await this._data.TagFriendsInPosts
                 .Where(p => p.PostId == postId)
                 .ToListAsync();

            this._data.TagFriendsInPosts.RemoveRange(entities);
            await this._data.SaveChangesAsync();
        }

        public async Task DeleteTaggedFriendsCommentId(int commentId)
        {
            var entities = await this._data.TagFriendsInComments
                 .Where(c => c.CommentId == commentId)
                 .ToListAsync();

            this._data.TagFriendsInComments.RemoveRange(entities);
            await this._data.SaveChangesAsync();
        }

        public async Task DeleteTaggedFriendsInComments(ICollection<int> commentsIds)
        {
            var entities = await this._data.TagFriendsInComments
                .Where(i => commentsIds
                        .Contains((int)i.CommentId))
                .ToListAsync();

            if (entities.Count > 0)
            {
                this._data.TagFriendsInComments.RemoveRange(entities);
                await this._data.SaveChangesAsync();
            }
        }

        public async Task UpdateTaggedFriendsInPostAsync(
            IList<UserServiceModel> taggedFriends,
            int postId,
            string taggerId)
        {
            //Get tag friends entities
            var tagFriendsEntities = await this._data.TagFriendsInPosts
                .Where(t => t.PostId == postId &&
                        t.TaggerId == taggerId)
                .ToListAsync();

            var taggedFriendsIds = GetUniqueIds(taggedFriends);
        
            for (int i = 0; i < tagFriendsEntities.Count; i++)
            {
                //This action shows that the current friend is not untagged/modified.
                if (taggedFriendsIds.Any(id =>id == tagFriendsEntities[i].TaggedId))
                {
                    var taggedFriendIndex = GetTaggedFriendIndex(
                        taggedFriendsIds.ToList(),
                        tagFriendsEntities[i].TaggedId);

                    taggedFriendsIds.RemoveAt(taggedFriendIndex);
                }
                //This action shows that the current friend is untagged/modified.
                else if (!taggedFriendsIds.Any(id => id == tagFriendsEntities[i].TaggedId))
                {
                    await RemoveTaggedFriendPost(tagFriendsEntities[i].TaggedId, postId);
                }
            }

            //This action check for newly tagged friends
            if (taggedFriendsIds.Count > 0)
            {
                foreach (var taggedId in taggedFriendsIds)
                {
                    await TagFriendPost(taggerId, taggedId, postId);
                }
            }
        }

        public async Task UpdateTaggedFriendsInCommentAsync(
            IList<UserServiceModel> taggedFriends,
            int commentId,
            string taggerId)

        {
            //Get tag friends entities
            var tagFriendsEntities = await this._data.TagFriendsInComments
                .Where(t => t.CommentId == commentId &&
                        t.TaggerId == taggerId)
                .ToListAsync();

            var taggedFriendsIds = GetUniqueIds(taggedFriends);

            for (int i = 0; i < tagFriendsEntities.Count; i++)
            {
                //This action shows that the current friend is not untagged/modified.
                if (taggedFriendsIds.Any(id => id == tagFriendsEntities[i].TaggedId))
                {
                    var taggedFriendIndex = GetTaggedFriendIndex(
                        taggedFriendsIds.ToList(),
                        tagFriendsEntities[i].TaggedId);
                    taggedFriendsIds.RemoveAt(taggedFriendIndex);
                }
                //This action shows that the current friend is untagged/modified.
                else if (!taggedFriendsIds.Any(id => id == tagFriendsEntities[i].TaggedId))
                {
                    await RemoveTaggedFriendComment(tagFriendsEntities[i].TaggedId, commentId);
                }
            }

            //This action check for newly tagged friends
            if (taggedFriendsIds.Count > 0)
            {
                foreach (var taggedId in taggedFriendsIds)
                {
                    await TagFriendComment(taggerId, taggedId, commentId);
                }
            }
        }

        private int GetTaggedFriendIndex(List<string> usersIds, string taggedId)
        {
            for (int i = 0; i < usersIds.Count; i++)
            {
                if (usersIds[i] == taggedId)
                {
                    return i;
                }
            }
            return -1;
        }

        private IList<string> GetUniqueIds(IList<UserServiceModel> taggedFriends)
        => taggedFriends.Select(i => i.Id).Distinct().ToList();
    }
}
