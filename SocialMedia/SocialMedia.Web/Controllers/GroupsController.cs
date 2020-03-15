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

namespace SocialMedia.Web.Controllers
{
    public class GroupsController : Controller
    {
        private readonly SocialMediaDbContext _context;
        private readonly UserManager<User> _userManager;

        public GroupsController(SocialMediaDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        //TODO: Group page
        //TODO: Add posts to every group

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            //Gets the current user
            var user = await this._userManager.GetUserAsync(User);
            
            //Gets all data from mapping table UsersInGroups of the current user
            var userGroups = await this._context.UsersInGroups
                .Where(i =>i.UserId == user.Id)
                .ToListAsync();

            //Gets all Groups
            var groups = await this._context.Groups.ToListAsync();
            
            //Assign all groups to collection where the current user is not a member
            var nonMemberGroups = groups;

            //Compare all groups with these groups where the current user is already a member or admin
            foreach (var group in userGroups)
            {
                //Gets the group where the user is already a member
                var memberGroup = groups.Find(i =>i.GroupId == group.GroupId);
                //Removes the group where the current user is already a member
                nonMemberGroups.Remove(memberGroup);
            }

            return View(nonMemberGroups);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Groups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,Title,Description")] Group group)
        {
            //TODO: groups validations
            if (ModelState.IsValid)
            {
                //Gets the current user 
                var user = await this._userManager.GetUserAsync(User);
                
                //Create new row in the UsersInGroups table where the current user is admin
                var newGroup = new UserInGroup()
                {
                    UserId = user.Id,
                    User = user,
                    GroupId = group.GroupId,
                    Group = group,
                    Admin = true
                };

                await this._context.UsersInGroups.AddAsync(newGroup);
                await this._context.Groups.AddAsync(group);
                await _context.SaveChangesAsync();
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

        //Get: Groups/MyGroups
        public async Task<IActionResult> MyGroups() 
        {
            //Gets the current user
            var user = await this._userManager.GetUserAsync(User);
            
            //Gets all data from the mapping table UsersInGroups for the current user
            var groupsIds = await this._context.UsersInGroups
                .Where(id => id.UserId == user.Id)
                .ToListAsync();
            //All groups of the current user
            var groups = new List<Group>();
            foreach (var groupId in groupsIds)
            {
                //Gets the group which match with the groupId from UsersInGroups table
                var group = await this._context.Groups
                    .FirstOrDefaultAsync(i => i.GroupId == groupId.GroupId);
                
                //Check is the user is admin 
                //If true set admin in the Message prop of the group
                if (groupId.Admin == true)
                {
                    group.Message = "admin";
                }

                groups.Add(group); 
            }
            return View(groups);
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

            return RedirectToAction("Index");
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}
