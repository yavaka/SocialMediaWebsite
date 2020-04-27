using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> NonGroupMemberDetails(int? id)
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

            var group = await _context.Groups
                .Include(p => p.Posts)
                .FirstOrDefaultAsync(m => m.GroupId == id);

            if (group == null)
            {
                return NotFound();
            }

            return View(group);
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

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,Title,Description")] Group group)
        {
            if (id != group.GroupId)
            {
                return NotFound();
            }

            //Unique title
            if (await this._context.Groups.AnyAsync(i => i.Title == group.Title))
            {
                ModelState.AddModelError("Title", $"Title {group.Title} already exists. Title must be unique!");
                return View();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.GroupId))
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
            return View(group);
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
            var group = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MyGroups));
        }

        //Post: Groups/JoinGroup
        public async Task<IActionResult> JoinGroup(int id)
        {
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
                GroupId = id,
                Group = group
            };

            this._context.UsersInGroups.Add(joinGroup);
            await this._context.SaveChangesAsync();

            return RedirectToAction("MyGroups");
        }

        //Get: Groups/GroupMembers/
        public async Task<IActionResult> GroupMembers(int id)
        {
            var user = await this._userManager.GetUserAsync(User);
            //Gets all of the members in the group with id from UsersInGroups table
            var membersInTheGroup = await this._context.UsersInGroups
                .Where(gId => gId.GroupId == id)
                .ToListAsync();

            var members = new List<User>();
            foreach (var member in membersInTheGroup)
            {
                if (member.UserId != user.Id)
                {
                    members.Add(await this._context.Users
                    .FirstOrDefaultAsync(i => i.Id == member.UserId));
                }
            }

            return View(members);
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}
