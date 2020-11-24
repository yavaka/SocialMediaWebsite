namespace SocialMedia.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using SocialMedia.Services.Comment;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Services.JSON;
    using SocialMedia.Services.Post;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.User;
    using SocialMedia.Web.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [Authorize]
    public class PostsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IPostService _postService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IJsonService<UserServiceModel> _jsonService;

        public PostsController(
            IFriendshipService friendshipService,
            IPostService postService,
            ITaggedUserService taggedUserService,
            ICommentService commentService,
            IUserService userService,
            IJsonService<UserServiceModel> jsonService)
        {
            this._friendshipService = friendshipService;
            this._postService = postService;
            this._taggedUserService = taggedUserService;
            this._commentService = commentService;
            this._userService = userService;
            this._jsonService = jsonService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? groupId, string path)
        {
            //If the page is reloaded without any usage of TempData,
            //it will be cleared before add a new key value pair.
            TempData.Clear();
            if (path != null)
            {
                TempData["path"] = path;
            }

            var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            var viewModel = new PostViewModel
            {
                TagFriends = new TagFriendsServiceModel()
                {
                    Friends = await this._friendshipService
                    .GetFriendsAsync(currentUserId),
                    TaggedFriends = new List<UserServiceModel>()
                },
                GroupId = groupId
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await this._userService
                    .GetUserByNameAsync(User.Identity.Name);

                await this._postService
                    .AddPost(new PostServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        GroupId = viewModel.GroupId,
                        TaggedFriends = viewModel.TaggedFriends != null ?
                            this._jsonService
                                .GetObjects(viewModel.TaggedFriends)
                                .ToList() :
                            new List<UserServiceModel>()
                    });

                if (TempData.ContainsKey("path"))
                {
                    return LocalRedirect(TempData["path"].ToString());
                }
                else
                {
                    return NotFound();
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, string path)
        {
            //If the page is reloaded without any usage of TempData,
            //it will be cleared before add a new key value pair.
            TempData.Clear();
            if (path != null)
            {
                TempData["path"] = path;
            }

            var post = await this._postService.GetPost(id);

            var currentUser = await this._userService
                .GetUserByNameAsync(User.Identity.Name);

            if (post == null ||
                currentUser.Id != post.Author.Id)
            {
                return NotFound();
            }

            var viewModel = new PostViewModel
            {
                PostId = post.PostId,
                Content = post.Content,
                GroupId = post.GroupId,
                Author = post.Author,
            };

            var friends = await this._friendshipService
                .GetFriendsAsync(viewModel.Author.Id);

            if (post.TaggedFriends.Count > 0)
            {
                viewModel.TagFriends = new TagFriendsServiceModel
                {
                    Friends = this._taggedUserService
                        .GetUntaggedFriends(post.TaggedFriends.ToList(), friends.ToList())
                        .ToList(),
                    TaggedFriends = new List<UserServiceModel>()
                };
            }
            else
            {
                viewModel.TagFriends = new TagFriendsServiceModel
                {
                    Friends = friends,
                    TaggedFriends = new List<UserServiceModel>()
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

                viewModel.TagFriends.TaggedFriends = viewModel.TagFriends.Friends
                    .Where(c => c.Checked == true)
                    .ToList();

                await this._taggedUserService.UpdateTaggedFriendsInPostAsync(
                    viewModel.TagFriends.TaggedFriends,
                    viewModel.PostId,
                    currentUserId);

                await this._postService
                    .EditPost(new PostServiceModel
                    {
                        PostId = viewModel.PostId,
                        Content = viewModel.Content
                    });

                if (TempData.ContainsKey("path"))
                {
                    return Redirect(TempData["path"].ToString());
                }
                else
                {
                    return NotFound();
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, string path)
        {
            //If the page is reloaded without any usage of TempData,
            //it will be cleared before add a new key value pair.
            TempData.Clear();
            if (path != null)
            {
                TempData["path"] = path;
            }

            var post = await this._postService
                .GetPost(id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //If groupId is not null it will be redirected to Group/Details/{groupId}
            var groupId = await this._postService.GetGroupIdOfPost(id);

            var comments = await this._commentService
                .GetCommentsByPostIdAsync(id);

            //Send collection of all comments ids and delete the tagged friends 
            await this._taggedUserService.DeleteTaggedFriendsInComments(comments
                .Select(i => i.CommentId)
                .ToList());

            await this._taggedUserService.DeleteTaggedFriendsPostId(id);
            await this._postService.DeletePost(id);


            if (TempData.ContainsKey("path"))
            {
                return Redirect(TempData["path"].ToString());
            }
            else
            {
                return NotFound();
            }
        }
    }
}
