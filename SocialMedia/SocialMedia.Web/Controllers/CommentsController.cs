namespace SocialMedia.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Web.Models;
    using SocialMedia.Services.Friendship;
    using SocialMedia.Services.Comment;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.User;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    using SocialMedia.Services.JSON;

    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ICommentService _commentService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly IUserService _userService;
        private readonly IJsonService<UserServiceModel> _jsonService;

        public CommentsController(
            IFriendshipService friendshipService,
            ICommentService commentService,
            ITaggedUserService taggedUserService,
            IUserService userService,
            IJsonService<UserServiceModel> jsonService)
        {
            this._friendshipService = friendshipService;
            this._commentService = commentService;
            this._taggedUserService = taggedUserService;
            this._userService = userService;
            this._jsonService = jsonService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int postId, string path)
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

            var viewModel = new CommentViewModel
            {
                PostId = postId,
                TagFriends = new TagFriendsServiceModel()
                {
                    Friends = await this._friendshipService
                        .GetFriendsAsync(currentUserId),
                    TaggedFriends = new List<UserServiceModel>()
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await this._userService
                    .GetUserByNameAsync(User.Identity.Name);

                await this._commentService
                    .AddComment(new CommentServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        PostId = viewModel.PostId,
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

            var comment = await this._commentService
                .GetComment(id);

            var currentUser = await this._userService
                .GetUserByNameAsync(User.Identity.Name);

            if (comment == null ||
                currentUser.Id != comment.Author.Id)
            {
                return NotFound();
            }

            var viewModel = new CommentViewModel
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                DatePosted = comment.DatePosted,
                Author = comment.Author,
                PostId = comment.PostId
            };

            var friends = await this._friendshipService
                .GetFriendsAsync(viewModel.Author.Id);

            if (comment.TaggedFriends.Count > 0)
            {
                viewModel.TagFriends = new TagFriendsServiceModel
                {
                    Friends = this._taggedUserService
                        .GetUntaggedFriends(comment.TaggedFriends.ToList(), friends.ToList())
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
        public async Task<IActionResult> Edit(CommentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

                viewModel.TagFriends.TaggedFriends = viewModel.TagFriends.Friends
                    .Where(c => c.Checked == true)
                    .ToList();

                await this._taggedUserService.UpdateTaggedFriendsInCommentAsync(
                    viewModel.TagFriends.TaggedFriends,
                    viewModel.CommentId,
                    currentUserId);

                await this._commentService
                    .EditComment(new CommentServiceModel
                    {
                        CommentId = viewModel.CommentId,
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

            var comment = await this._commentService
                .GetComment((int)id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await this._taggedUserService.DeleteTaggedFriendsCommentId(id);
            await this._commentService.DeleteComment(id);

            if (TempData.ContainsKey("path"))
            {
                return Redirect(TempData["path"].ToString());
            }
            return NotFound();
        }
    }
}
