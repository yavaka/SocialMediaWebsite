namespace SocialMedia.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Web.Models;
    using SocialMedia.Services.Post;
    using System;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.Comment;
    using System.Linq;
    using SocialMedia.Services.User;
    using SocialMedia.Services.Url;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class PostsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly IPostService _postService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IUrlService _urlService;

        public PostsController(
            IFriendshipService friendshipService,
            IPostService postService,
            ITaggedUserService taggedUserService,
            ICommentService commentService,
            IUserService userService,
            IUrlService urlService)
        {
            this._friendshipService = friendshipService;
            this._postService = postService;
            this._taggedUserService = taggedUserService;
            this._commentService = commentService;
            this._userService = userService;
            this._urlService = urlService;
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

            var currentUserId = this._userService
                .GetUserId(User);

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
                    .GetCurrentUserAsync(User);

                //Get tagged friends
                if (viewModel.TagFriends.Friends.Any(c => c.Checked == true))
                {
                    viewModel.TagFriends.TaggedFriends = viewModel.TagFriends.Friends
                        .Where(c => c.Checked == true)
                        .ToList();
                }

                await this._postService
                    .AddPost(new PostServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        GroupId = viewModel.GroupId,
                        TaggedFriends = viewModel.TagFriends.TaggedFriends
                    });

                if (TempData.ContainsKey("path"))
                {
                    var returnUrl = this._urlService
                        .GenerateReturnUrl(TempData["path"].ToString(), HttpContext);
                    return Redirect(returnUrl);
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
                .GetCurrentUserAsync(User);

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
                var currentUserId = this._userService.GetUserId(User);

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
                    var returnUrl = this._urlService
                        .GenerateReturnUrl(TempData["path"].ToString(), HttpContext);
                    return Redirect(returnUrl);
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
                var returnUrl = this._urlService
                    .GenerateReturnUrl(TempData["path"].ToString(), HttpContext);
                return Redirect(returnUrl);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
