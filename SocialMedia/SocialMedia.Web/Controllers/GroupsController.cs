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
                .GetCurrentUserAsync(User);

            var nonMemberGroups = await this._groupService
                .GetNonMemberGroupsAsync(currentUser);

            return View(nonMemberGroups);
        }

        [HttpGet]
        public async Task<IActionResult> JoinedGroups()
        {
            var currentUser = await this._userService
               .GetCurrentUserAsync(User);

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

                serviceModel.AdminId = this._userService
                    .GetUserId(User);

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

            var currentUserId = this._userService
                .GetUserId(User);

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
            var currentUserId = this._userService
                .GetUserId(User);

            await this._groupService.JoinGroupAsync(id, currentUserId);

            return RedirectToAction(nameof(Details), new { id = id });
        }

        public async Task<IActionResult> Leave(int id)
        {
            var currentUserId = this._userService
                .GetUserId(User);

            await this._groupService.LeaveGroupAsync(id, currentUserId);

            return RedirectToAction(nameof(NonMemberGroups));
        }

        //// GET: Groups/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var group = await _context.Groups
        //        .FirstOrDefaultAsync(m => m.GroupId == id);
        //    if (group == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(group);
        //}

        //// POST: Groups/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var group = await _context.Groups
        //        .Include(p => p.Posts)
        //        .FirstOrDefaultAsync(i => i.GroupId == id);

        //    //Remove all group posts  
        //    if (group.Posts.Count > 0)
        //    {
        //        RemoveGroupPosts(group.GroupId);
        //    }

        //    _context.Groups.Remove(group);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction(nameof(MyGroups));
        //}

        //private bool GroupExists(int id)
        //{
        //    return _context.Groups.Any(e => e.GroupId == id);
        //}

        //#endregion

        ////Post: Groups/JoinGroup
        //public async Task<IActionResult> JoinGroup(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //Gets the current user
        //    var user = await this._userManager.GetUserAsync(User);

        //    //Gets the group with id sent from Groups/Details view
        //    var group = await this._context.Groups
        //        .FirstOrDefaultAsync(i => i.GroupId == id);

        //    //Adds new row in the UsersIngroups table where the current user is not admin
        //    var joinGroup = new UserInGroup()
        //    {
        //        UserId = user.Id,
        //        User = user,
        //        GroupId = (int)id,
        //        Group = group
        //    };

        //    this._context.UsersInGroups.Add(joinGroup);
        //    await this._context.SaveChangesAsync();

        //    return RedirectToAction("MyGroups");
        //}

        //public async Task<IActionResult> LeaveGroup(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //Gets the current user
        //    var user = await this._userManager.GetUserAsync(User);

        //    var userInGroupEntity = await this._context.UsersInGroups
        //        .FirstOrDefaultAsync(i => i.GroupId == id &&
        //                                  i.UserId == user.Id);

        //    this._context.UsersInGroups.Remove(userInGroupEntity);
        //    this._context.SaveChanges();

        //    return RedirectToAction(nameof(MyGroups));
        //}

        ////Get: Groups/GroupMembers
        //public async Task<IActionResult> GroupMembers(int id)
        //{
        //    var user = await this._userManager.GetUserAsync(User);
        //    //Gets all of the members in the group with id from UsersInGroups table
        //    var membersInTheGroup = await this._context.UsersInGroups
        //        .Include(u => u.User)
        //        .Where(gId => gId.GroupId == id)
        //        .ToListAsync();

        //    var members = new List<User>();
        //    foreach (var member in membersInTheGroup)
        //    {
        //        if (member.UserId != user.Id)
        //        {
        //            if (member.Admin == true)
        //            {
        //                member.User.Message = "admin";
        //            }
        //            members.Add(member.User);
        //        }
        //    }

        //    return View(members);
        //}

    }
}
