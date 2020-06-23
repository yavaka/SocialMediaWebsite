using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models;
using SocialMedia.Models.ViewModels;

namespace SocialMedia.Web.Controllers
{
    public class GroupsController : Controller
    {
        private readonly SocialMediaDbContext _context;
        private readonly UserManager<User> _userManager;
        private static GroupViewModel ViewModel = new GroupViewModel();
        public GroupsController(SocialMediaDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        #region CRUD
        // GET: Groups
        public async Task<IActionResult> Index()
        {
            ViewModel = new GroupViewModel();
            //Gets the current user
            ViewModel.CurrentUser = await this._userManager.GetUserAsync(User);

            //Gets all data from mapping table UsersInGroups of the current user
            var userGroups = await this._context.UsersInGroups
                .Where(i => i.UserId == ViewModel.CurrentUser.Id)
                .ToListAsync();

            //Gets all Groups
            var groups = await this._context.Groups.ToListAsync();

            //Assign all groups to collection where the current user is not a member
            ViewModel.NonMemberGroups = groups;

            //Compare all groups with these groups where the current user is already a member or admin
            foreach (var group in userGroups)
            {
                //Gets the group where the user is already a member
                var memberGroup = groups.Find(i => i.GroupId == group.GroupId);
                //Removes the group where the current user is already a member
                ViewModel.NonMemberGroups.Remove(memberGroup);
            }

            return View(ViewModel);
        }

        //Get: Groups/MyGroups
        public async Task<IActionResult> MyGroups()
        {
            ViewModel = new GroupViewModel();

            //Gets the current user
            ViewModel.CurrentUser = await this._userManager.GetUserAsync(User);

            //Gets all data from the mapping table UsersInGroups for the current user
            var userGroups = await this._context.UsersInGroups
                .Where(id => id.UserId == ViewModel.CurrentUser.Id)
                .Include(g => g.Group)
                .ToListAsync();

            foreach (var userGroup in userGroups)
            {
                //Check is the user is admin 
                //If true set admin in the Message prop of the group
                if (userGroup.Admin == true)
                {
                    userGroup.Group.Message = "admin";
                }

                ViewModel.MemberGroups.Add(userGroup.Group);
            }
            return View(ViewModel);
        }

        // GET: Groups/NonGroupMemberDetails
        public async Task<IActionResult> NonMemberDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupId == id);

            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewModel = new GroupViewModel();
            
            //Gets the group
            ViewModel.Group = await _context.Groups
                .Include(p =>p.Posts)
                .FirstOrDefaultAsync(m => m.GroupId == id);

            if (ViewModel.Group == null)
            {
                return NotFound();
            }

            if (ViewModel.Group.Posts.Count > 0)
            {
                //Iterate the group`s posts
                foreach (var post in ViewModel.Group.Posts)
                {
                    //Gets the current post with TagFriends and Author db records
                    var postTFEntities = GetPostById(post.PostId);
                    ViewModel.Posts.Add(new PostTagFriendsViewModel() 
                    {
                        Post = postTFEntities,
                        Tagged = (postTFEntities.TaggedUsers.Count > 0) 
                                    ? await GetTaggedUsersAsync(postTFEntities.TaggedUsers) 
                                    : new List<User>(),
                    });
                }
            }
            
            return View(ViewModel);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]GroupViewModel viewModel)
        {
            if (viewModel.Group == null)
            {
                return NotFound();
            }
            else
            {
                ViewModel.Group = viewModel.Group;
            }

            //Unique title
            if (await this._context.Groups.AnyAsync(i => i.Title == ViewModel.Group.Title))
            {
                ModelState.AddModelError("Title", $"Title {ViewModel.Group.Title} already exists. Title must be unique!");
                return View(ViewModel);
            }

            if (ModelState.IsValid)
            {
                //Gets the current user 
                ViewModel.CurrentUser = await this._userManager.GetUserAsync(User);

                //Create new row in the UsersInGroups table where the current user is admin
                var adminGroup = new UserInGroup()
                {
                    UserId = ViewModel.CurrentUser.Id,
                    User = ViewModel.CurrentUser,
                    GroupId = ViewModel.Group.GroupId,
                    Group = ViewModel.Group,
                    Admin = true
                };

                await this._context.UsersInGroups.AddAsync(adminGroup);
                await this._context.Groups.AddAsync(ViewModel.Group);
                await _context.SaveChangesAsync();
                ViewModel = new GroupViewModel();
                return RedirectToAction(nameof(MyGroups));
            }
            return View();
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewModel = new GroupViewModel();
            ViewModel.Group = await _context.Groups
                .FirstOrDefaultAsync(i => i.GroupId == id);

            if (ViewModel.Group == null)
            {
                return NotFound();
            }
            return View(ViewModel);
        }

        // POST: Groups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GroupViewModel viewModel)
        {
            if (viewModel.Group == null)
            {
                return NotFound();
            }
            //If viewModel parameter has a different title than in a connected record,
            //will be assigned to the connected record
            if (viewModel.Group.Title != ViewModel.Group.Title)
            {
                //Unique title
                if (await this._context.Groups.AnyAsync(i => i.Title == viewModel.Group.Title))
                {
                    ModelState.AddModelError("Title", $"Title {viewModel.Group.Title} already exists. Title must be unique!");
                    return View();
                }
                else
                {
                    ViewModel.Group.Title = viewModel.Group.Title;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(viewModel.Group.GroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(MyGroups));
            }
            return View(viewModel);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.Groups
                .Include(p => p.Posts)
                .FirstOrDefaultAsync(i =>i.GroupId == id);

            //Remove all group posts  
            if (group.Posts.Count > 0)
            {
                RemoveGroupPosts(group.GroupId);
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
           
            return RedirectToAction(nameof(MyGroups));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }

        #endregion

        //Post: Groups/JoinGroup
        public async Task<IActionResult> JoinGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Gets the current user
            var user = await this._userManager.GetUserAsync(User);

            //Gets the group with id sent from Groups/Details view
            var group = await this._context.Groups
                .FirstOrDefaultAsync(i => i.GroupId == id);

            //Adds new row in the UsersIngroups table where the current user is not admin
            var joinGroup = new UserInGroup()
            {
                UserId = user.Id,
                User = user,
                GroupId = (int)id,
                Group = group
            };

            this._context.UsersInGroups.Add(joinGroup);
            await this._context.SaveChangesAsync();

            return RedirectToAction("MyGroups");
        }

        public async Task<IActionResult> LeaveGroup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Gets the current user
            var user = await this._userManager.GetUserAsync(User);

            var userInGroupEntity = await this._context.UsersInGroups
                .FirstOrDefaultAsync(i => i.GroupId == id &&
                                          i.UserId == user.Id);

            this._context.UsersInGroups.Remove(userInGroupEntity);
            this._context.SaveChanges();

            return RedirectToAction(nameof(MyGroups));
        }

        //Get: Groups/GroupMembers
        public async Task<IActionResult> GroupMembers(int id)
        {
            var user = await this._userManager.GetUserAsync(User);
            //Gets all of the members in the group with id from UsersInGroups table
            var membersInTheGroup = await this._context.UsersInGroups
                .Include(u => u.User)
                .Where(gId => gId.GroupId == id)
                .ToListAsync();

            var members = new List<User>();
            foreach (var member in membersInTheGroup)
            {
                if (member.UserId != user.Id)
                {
                    if (member.Admin == true)
                    {
                        member.User.Message = "admin";
                    }
                    members.Add(member.User);
                }
            }

            return View(members);
        }

        //TODO: Tag friends service: GetTaggedUsersByPostId(int postId)
        private async Task<ICollection<User>> GetTaggedUsersAsync(ICollection<TagFriends> tagFriendEntities)
        {
            //Gets the tagged users
            var taggedUsers = new List<User>();
            foreach (var tagged in tagFriendEntities)
            {
                taggedUsers.Add(await this._userManager.FindByIdAsync(tagged.TaggedId));
            }

            return taggedUsers;
        }

        //TODO: Posts service: GetPostById(int postId)
        private Post GetPostById(int postId)
        {
            return this._context.Posts
                .Include(tf => tf.TaggedUsers)
                .Include(a => a.Author)
                .Include(c => c.Comments)
                .FirstOrDefault(i => i.PostId == postId);
        }

        private void RemoveGroupPosts(int groupId)
        {
            //Connected posts
            var posts = this._context.Posts
                .Include(t => t.TaggedUsers)
                .Include(c => c.Comments)
                .Where(g => g.GroupId == groupId);

            //Get posts`s comments in multi-dimentional collection
            var postsComments = posts.Select(c => c.Comments).ToList();

            /// Parameter: convert the collection to 1d list
            /// Remove comments` TagFriends entites without comments because on delete is set to cascade
            RemoveCommentsTagFriendsEntities(postsComments.SelectMany(c => c.Distinct()));


            //Multi-dimensional collection of TagFriends
            var postsTagFriendEntities = posts.Select(t => t.TaggedUsers).ToList();

            //Convert to 1d list and remove
            this._context.TagFriends.RemoveRange(
                postsTagFriendEntities.SelectMany(c => c.Distinct()));
        }

        private void RemoveCommentsTagFriendsEntities(IEnumerable<Comment> comments)
        {
            foreach (var comment in comments)
            {
                //Get comment`s TagFriends entities
                comment.TaggedUsers = this._context.TagFriends
                    .Where(cId => cId.CommentId == comment.Id)
                    .ToList();

                this._context.TagFriends.RemoveRange(comment.TaggedUsers);
            }
        }
    }
}
