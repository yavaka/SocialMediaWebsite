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
    //TODO: Integrate tag friends functionality in Create and Edit methods
    public class CommentsController : Controller
    {
        private readonly SocialMediaDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        private static int _postId = 0;
        private static CommentTagFriendsViewModel ViewModel = new CommentTagFriendsViewModel();

        public CommentsController(SocialMediaDbContext context,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        #region CRUD

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
            var userId = this._userManager.GetUserId(User);
            var user = this._context.Users.FirstOrDefault(i => i.Id == userId);

            ViewModel = new CommentTagFriendsViewModel()
            {
                CurrentUser = user,
                UserFriends = GetUserFriends(user)
            };

            return View(ViewModel);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]CommentTagFriendsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                //Get current user
                var user = await this._userManager.GetUserAsync(User);
                ViewModel.CurrentUser = user;
                var comment = new Comment()
                {
                    Author = user,
                    AuthorId = user.Id,
                    DatePosted = DateTime.Now,
                    Content = viewModel.Comment.Content,
                    TaggedUsers = TagFriendEntities()

                };

                //Get the post
                var post = await this._context.Posts.FirstOrDefaultAsync(i => i.PostId == _postId);

                if (post == null)
                {
                    return NotFound();
                }

                comment.CommentedPost = post;
                comment.CommentedPostId = post.PostId;

                this._context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                ViewModel = new CommentTagFriendsViewModel();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
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

        #endregion

        //TODO:Friendship service : GetUserFriends
        private List<User> GetUserFriends(User currentUser)
        {
            //Get current user friendships where it is addressee or requester
            var friendships = this._context.Friendships
                .Where(i => i.AddresseeId == currentUser.Id && i.Status == 1 ||
                            i.RequesterId == currentUser.Id && i.Status == 1)
                .Include(i => i.Addressee)
                .Include(i => i.Requester)
                .ToList();

            var friends = new List<User>();
            //Add all friends
            foreach (var friendship in friendships)
            {
                //If addressee is the current user, add requester if it is not already tagged
                if (friendship.AddresseeId == currentUser.Id &&
                    !ViewModel.Tagged.Contains(friendship.Requester))
                {
                    /*this._context.Users
                        .FirstOrDefault(i => i.Id == friendship.RequesterId)*/
                    friends.Add(friendship.Requester);
                }
                else if (friendship.RequesterId == currentUser.Id &&
                    !ViewModel.Tagged.Contains(friendship.Addressee)) //If requester is current user, add addressee
                {
                    //this._context.Users
                    //    .FirstOrDefault(i => i.Id == friendship.AddresseeId)
                    friends.Add(friendship.Addressee);
                }
            }

            return friends;
        }

        //TODO: Tag friends service : TagFriend(string taggedId, string taggerId)
        public IActionResult TagFriend(string taggedId, string viewName)
        {
            //TODO Bug : When the view is reloaded adds a duplicate item in the ViewModel.Tagged collection 

            //Adds tagged user in tagged collection of the view model
            var taggedUser = this._context.Users
                .FirstOrDefault(i => i.Id == taggedId);

            ViewModel.Tagged.Add(taggedUser);

            //Remove tagged user from user friends list of view model
            //The concept is that if any of the tagged users exist in this collection, 
            //it does not make sense to be tagged twice in one post.
            var item = ViewModel.UserFriends.SingleOrDefault(i => i.Id == taggedUser.Id);
            ViewModel.UserFriends.Remove(item);

            //viewName variable is used to determine where the form comes from
            return View(viewName, ViewModel);
        }

        //TODO: Tag friends service : Entities
        private ICollection<TagFriends> TagFriendEntities()
        {
            var tagFriendsEntities = new List<TagFriends>();
            foreach (var tagged in ViewModel.Tagged)
            {
                var tagFriendsEntity = new TagFriends()
                {
                    TaggerId = ViewModel.CurrentUser.Id,
                    TaggedId = tagged.Id,
                };
                tagFriendsEntities.Add(tagFriendsEntity);
            }
            return tagFriendsEntities;
        }
    }
}
