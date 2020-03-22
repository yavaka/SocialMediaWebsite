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
    public class PostsController : Controller
    {
        private readonly SocialMediaDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private static int _groupId = 0;



        public PostsController(SocialMediaDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //TODO: Service for posts example: GetAll(), FindById(), etc.
        
        // GET: GroupPosts
        public async Task<IActionResult> GroupPosts()
        {
            TempData["groupPost"] = "true";
            if (TempData["groupId"] != null)
            {
                _groupId = int.Parse(TempData["groupId"].ToString());
            }
            
            var user = await _userManager.GetUserAsync(User);
            var posts = await _context.Posts
                .Where(a => a.GroupId == _groupId)
                .ToListAsync();

            //If the current user is author of some post, it can be edited and deleted
            foreach (var post in posts)
            {
                if (post.AuthorId == user.Id)
                {
                    post.Message = "author";
                }
            }

            return View(posts);
        }

        // GET: UserPosts
        public async Task<IActionResult> UserPosts()
        {
            TempData["userPost"] = "true";
            var user = await _userManager.GetUserAsync(User);

            var posts = await _context.Posts
                .Where(a => a.AuthorId == user.Id && a.GroupId == null)
                .ToListAsync();

            return View(posts);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await this._userManager.GetUserAsync(User);

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            if (post.AuthorId == user.Id)
            {
                post.Message = "author";
            }

            //Pass current postId to CommentsController
            TempData["postId"] = id;

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Content")] Post post)
        {
            //TODO: Issue - when group post is created then user create a post doesnt list the post because _groupId is not 0
            //Creates group in the current user`s profile
            if (ModelState.IsValid && TempData["userPost"].ToString() == "true")
            {
                var user = await _userManager.GetUserAsync(User);

                post.Author = user;
                post.AuthorId = user.Id;
                post.DatePosted = DateTime.Now;
                _context.Posts.Add(post);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UserPosts));
            }
            //Creates post in a group
            else if (ModelState.IsValid && TempData["userGroup"].ToString() == "true")
            {
                var user = await _userManager.GetUserAsync(User);
                var group = await this._context.Groups
                    .FirstOrDefaultAsync(i => i.GroupId == _groupId);

                if (group == null)
                {
                    return NotFound();
                }

                post.Author = user;
                post.AuthorId = user.Id;
                post.Group = group;
                post.GroupId = group.GroupId;
                post.DatePosted = DateTime.Now;
                _context.Posts.Add(post);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GroupPosts));
            }
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post updatedPost)
        {
            if (id != updatedPost.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid && _groupId == 0)
            {
                try
                {
                    var user = await this._userManager.GetUserAsync(User);
                    var post = await this._context.Posts
                        .FirstOrDefaultAsync(i => i.PostId == updatedPost.PostId);

                    post.Author = user;
                    post.AuthorId = user.Id;
                    post.Content = updatedPost.Content;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(updatedPost.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(UserPosts));
            }
            else if (ModelState.IsValid && _groupId != 0)
            {
                try
                {
                    var user = await this._userManager.GetUserAsync(User);
                    var post = await this._context.Posts
                        .FirstOrDefaultAsync(i => i.PostId == updatedPost.PostId);
                    var group = await this._context.Groups
                    .FirstOrDefaultAsync(i => i.GroupId == _groupId);

                    if (group == null)
                    {
                        return NotFound();
                    }

                    post.Author = user;
                    post.AuthorId = user.Id;
                    post.Group = group;
                    post.GroupId = group.GroupId;
                    post.Content = updatedPost.Content;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(updatedPost.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GroupPosts));
            }
            
            return View(updatedPost);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserPosts));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
