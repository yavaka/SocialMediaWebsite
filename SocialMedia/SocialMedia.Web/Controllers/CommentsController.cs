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
    public class CommentsController : Controller
    {
        private readonly SocialMediaDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        private static int _postId = 0;

        public CommentsController(SocialMediaDbContext context,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            //Current postId
            if (TempData["postId"] != null)
            {
                _postId = int.Parse(TempData["postId"].ToString());
            }

            var user = await this._userManager.GetUserAsync(User);

            var comments = await _context.Comments
                .Where(a => a.CommentedPostId == _postId)
                .ToListAsync();

            //If the current user is author of some comment, it can be edited and deleted
            foreach (var comment in comments)
            {
                if (comment.AuthorId == user.Id)
                {
                    comment.Message = "author";
                }
            }

            return View(comments);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            if (ModelState.IsValid)
            {
                //Get current user
                var user = await this._userManager.GetUserAsync(User);

                //Sets current user as a author
                comment.Author = user;
                comment.AuthorId = user.Id;

                //Get the post
                var post = await this._context.Posts.FirstOrDefaultAsync(i => i.PostId == _postId);

                if (post == null)
                {
                    return NotFound();
                }

                comment.CommentedPost = post;
                comment.CommentedPostId = post.PostId;
                comment.DatePosted = DateTime.Now;

                this._context.Comments.Add(comment);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comment updatedComment)
        {
            if (id != updatedComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await this._userManager
                        .GetUserAsync(User);
                    var comment = await this._context.Comments
                        .FirstOrDefaultAsync(i => i.Id == id);
                    var post = await this._context.Posts
                        .FirstOrDefaultAsync(i => i.PostId == _postId);

                    comment.AuthorId = user.Id;
                    comment.Author = user;
                    comment.CommentedPostId = post.PostId;
                    comment.CommentedPost = post;
                    comment.Content = updatedComment.Content;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(updatedComment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(updatedComment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
