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
    using SocialMedia.Services.Url;
    using Microsoft.AspNetCore.Authorization;

    [Authorize]
    public class CommentsController : Controller
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ICommentService _commentService;
        private readonly ITaggedUserService _taggedUserService;
        private readonly IUserService _userService;
        private readonly IUrlService _urlService;

        public CommentsController(
            IFriendshipService friendshipService,
            ICommentService commentService,
            ITaggedUserService taggedUserService,
            IUserService userService,
            IUrlService urlService)
        {
            this._friendshipService = friendshipService;
            this._commentService = commentService;
            this._taggedUserService = taggedUserService;
            this._userService = userService;
            this._urlService = urlService;
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

            var currentUserId = this._userService
                .GetUserId(User);

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
                    .GetCurrentUserAsync(User);

                //Get tagged friends
                if (viewModel.TagFriends.Friends.Any(c => c.Checked == true))
                {
                    viewModel.TagFriends.TaggedFriends = viewModel.TagFriends.Friends
                        .Where(c => c.Checked == true)
                        .ToList();
                }

                await this._commentService
                    .AddComment(new CommentServiceModel
                    {
                        Content = viewModel.Content,
                        DatePosted = DateTime.Now,
                        Author = currentUser,
                        PostId = viewModel.PostId,
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

            var comment = await this._commentService
                .GetComment(id);

            var currentUser = await this._userService
                .GetCurrentUserAsync(User);

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
                var currentUserId = this._userService.GetUserId(User);

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
                var returnUrl = this._urlService
                    .GenerateReturnUrl(TempData["path"].ToString(), HttpContext);
                return Redirect(returnUrl);
            }
            return NotFound();
        }
    }
}
