namespace SocialMedia.Services.Post
{
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;
    using SocialMedia.Services.Comment;
    using SocialMedia.Services.Group;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.User;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class PostService : IPostService
    {
        private readonly SocialMediaDbContext _data;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;

        public PostService(
            SocialMediaDbContext data,
            ITaggedUserService taggedUserService,
            ICommentService commentService)
        {
            this._data = data;
            this._taggedUserService = taggedUserService;
            this._commentService = commentService;
        }

        public async Task AddPost(PostServiceModel serviceModel)
        {
            var post = new Post
            {
                Content = serviceModel.Content,
                DatePosted = serviceModel.DatePosted,
                AuthorId = serviceModel.Author.Id,
                TaggedUsers = this._taggedUserService.GetTagFriendsInPostsEntities(
                    serviceModel.Author.Id,
                    serviceModel.TaggedFriends
                        .Select(i => i.Id)
                        .ToList())
            };

            if (serviceModel.GroupId != null)
            {
                post.GroupId = serviceModel.GroupId;
            }

            await this._data.Posts.AddAsync(post);
            await this._data.SaveChangesAsync();
        }

        public async Task EditPost(PostServiceModel serviceModel)
        {
            var post = await this._data.Posts
                .FirstOrDefaultAsync(i => i.PostId == serviceModel.PostId);

            post.Content = serviceModel.Content;

            this._data.Update(post);
            await this._data.SaveChangesAsync();
        }

        public async Task DeletePost(int id)
        {
            var post = await this._data.Posts
                .FirstOrDefaultAsync(i => i.PostId == id);

            this._data.Posts.Remove(post);
            await this._data.SaveChangesAsync();
        }

        public async Task<PostServiceModel> GetPost(int id)
        => await this._data.Posts
            .Select(p => new PostServiceModel
            {
                PostId = p.PostId,
                Content = p.Content,
                DatePosted = p.DatePosted,
                Author = new UserServiceModel
                {
                    Id = p.Author.Id,
                    UserName = p.Author.UserName,
                    FullName = p.Author.FullName,
                    Country = p.Author.Country,
                    DateOfBirth = p.Author.DOB
                },
                GroupId = p.GroupId,
                Group = new GroupServiceModel
                {
                    Title = p.Group.Title,
                    Description = p.Group.Description
                },
                TaggedFriends = p.TaggedUsers
                    .Select(t => new UserServiceModel 
                    {
                        Id = t.Tagged.Id,
                        UserName = t.Tagged.UserName,
                        FullName = t.Tagged.FullName,
                        Country = t.Tagged.Country,
                        DateOfBirth = t.Tagged.DOB
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(i => i.PostId == id);

        public async Task<ICollection<PostServiceModel>> GetPostsByUserIdAsync(string userId)
        {
            var posts = this._data.Posts
                .Where(i => i.AuthorId == userId)
                .Select(p => new PostServiceModel
                {
                    PostId = p.PostId,
                    Content = p.Content,
                    DatePosted = p.DatePosted,
                    Author = new UserServiceModel
                    {
                        Id = p.Author.Id,
                        UserName = p.Author.UserName,
                        FullName = p.Author.FullName,
                        Country = p.Author.Country,
                        DateOfBirth = p.Author.DOB
                    },
                    GroupId = p.GroupId,
                    Group = new GroupServiceModel
                    {
                        Title = p.Group.Title,
                        Description = p.Group.Description
                    },
                    TaggedFriends = p.TaggedUsers
                        .Select(t => new UserServiceModel
                        {
                            Id = t.Tagged.Id,
                            UserName = t.Tagged.UserName,
                            FullName = t.Tagged.FullName,
                            Country = t.Tagged.Country,
                            DateOfBirth = t.Tagged.DOB
                        })
                        .ToList()
                })
                .ToList();

            foreach (var post in posts)
            {
                post.Comments = await this._commentService
                    .GetCommentsByPostIdAsync(post.PostId);
            }

            return posts;
        }

        public async Task<ICollection<PostServiceModel>> GetPostsByGroupIdAsync(int groupId)
        {
            var posts = this._data.Posts
               .Where(i => i.GroupId == groupId)
               .Select(p => new PostServiceModel
               {
                   PostId = p.PostId,
                   Content = p.Content,
                   DatePosted = p.DatePosted,
                   Author = new UserServiceModel
                   {
                       Id = p.Author.Id,
                       UserName = p.Author.UserName,
                       FullName = p.Author.FullName,
                       Country = p.Author.Country,
                       DateOfBirth = p.Author.DOB
                   },
                   TaggedFriends = p.TaggedUsers
                       .Select(t => new UserServiceModel
                       {
                           Id = t.Tagged.Id,
                           UserName = t.Tagged.UserName,
                           FullName = t.Tagged.FullName,
                           Country = t.Tagged.Country,
                           DateOfBirth = t.Tagged.DOB
                       })
                       .ToList()
               })
               .ToList();

            foreach (var post in posts)
            {
                post.Comments = await this._commentService
                    .GetCommentsByPostIdAsync(post.PostId);
            }

            return posts;
        }

        public async Task<int?> GetGroupIdOfPost(int id)
        {
            var post = await GetPost(id);

            return post.GroupId;
        }
    }
}
