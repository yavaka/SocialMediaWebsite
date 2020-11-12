namespace SocialMedia.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SocialMedia.Services.Comment;
    using SocialMedia.Services.Group;
    using SocialMedia.Services.Post;
    using SocialMedia.Services.TaggedUser;
    using SocialMedia.Services.User;
    using SocialMedia.Web.Models;

    [Authorize]
    public class GroupsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IGroupService _groupService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly ITaggedUserService _taggedUserService;

        public GroupsController(
            IUserService userService,
            IGroupService groupService,
            IPostService postService,
            ICommentService commentService,
            ITaggedUserService taggedUserService)
        {
            this._userService = userService;
            this._groupService = groupService;
            this._postService = postService;
            this._commentService = commentService;
            this._taggedUserService = taggedUserService;
        }

        [HttpGet]
        public async Task<IActionResult> NonMemberGroups()
        {
            var currentUser = await this._userService
                .GetUserByNameAsync(User.Identity.Name);

            var nonMemberGroups = await this._groupService
                .GetNonMemberGroupsAsync(currentUser);

            return View(nonMemberGroups);
        }

        [HttpGet]
        public async Task<IActionResult> JoinedGroups()
        {
            var currentUser = await this._userService
                .GetUserByNameAsync(User.Identity.Name);

            var joinedGroups = await this._groupService
                .GetJoinedGroupsAsync(currentUser);

            TempData["currentUserId"] = currentUser.Id;

            return View(joinedGroups);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupServiceModel serviceModel)
        {
            if (ModelState.IsValid)
            {
                //Unique title
                if (await this._groupService.IsTitleExistAsync(serviceModel.Title))
                {
                    ModelState.AddModelError(
                        "Title",
                        $"Title {serviceModel.Title} already exists. Title must be unique!");

                    return View(serviceModel);
                }

                serviceModel.AdminId = await this._userService
                    .GetUserIdByNameAsync(User.Identity.Name);

                await this._groupService.AddGroupAsync(serviceModel);

                return RedirectToAction(nameof(JoinedGroups));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviceModel = await this._groupService
                .GetGroupAsync(id);

            return View(serviceModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GroupServiceModel serviceModel)
        {
            if (ModelState.IsValid)
            {
                var groupTitle = this._groupService
                    .GetGroupTitle(serviceModel.GroupId);
                if (groupTitle != serviceModel.Title)
                {
                    //Unique title
                    if (await this._groupService.IsTitleExistAsync(serviceModel.Title))
                    {
                        ModelState.AddModelError(
                            "Title",
                            $"Title {serviceModel.Title} already exists. Title must be unique!");

                        return View(serviceModel);
                    }
                }

                await this._groupService.EditGroupAsync(serviceModel);

                return RedirectToAction(nameof(JoinedGroups));
            }
            return View(serviceModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var group = await this._groupService
                .GetGroupAsync(id);

            if (group == null)
            {
                return NotFound();
            }

            var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            var viewModel = new GroupViewModel
            {
                GroupId = group.GroupId,
                Title = group.Title,
                Description = group.Description,
                Admin = await this._userService
                    .GetUserByIdAsync(group.AdminId),
                Members = group.Members,
                Posts = await this._postService
                    .GetPostsByGroupIdAsync(group.GroupId),
                CurrentUserId = currentUserId,
                IsCurrentUserMember = await this._groupService
                    .IsCurrentUserMember(currentUserId, group.GroupId)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await this._groupService
                .GetGroupAsync(id);

            var viewModel = new GroupViewModel
            {
                GroupId = group.GroupId,
                Title = group.Title,
                Description = group.Description
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var posts = await this._postService
                .GetPostsByGroupIdAsync(id);

            foreach (var post in posts)
            {
                var comments = await this._commentService
                    .GetCommentsByPostIdAsync(post.PostId);

                //Send collection of all comments ids and delete the tagged friends 
                await this._taggedUserService.DeleteTaggedFriendsInComments(comments
                    .Select(i => i.CommentId)
                    .ToList());

                await this._taggedUserService.DeleteTaggedFriendsPostId(post.PostId);
            }

            await this._groupService.DeleteGroupAsync(id);

            return RedirectToAction(nameof(JoinedGroups));
        }

        public async Task<IActionResult> Join(int id)
        {
            var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            await this._groupService.JoinGroupAsync(id, currentUserId);

            return RedirectToAction(nameof(Details), new { id = id });
        }

        public async Task<IActionResult> Leave(int id)
        {
            var currentUserId = await this._userService
                .GetUserIdByNameAsync(User.Identity.Name);

            await this._groupService.LeaveGroupAsync(id, currentUserId);

            return RedirectToAction(nameof(NonMemberGroups));
        }
    }
}
