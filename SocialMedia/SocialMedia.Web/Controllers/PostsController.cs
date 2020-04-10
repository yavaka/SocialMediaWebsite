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
    //TODO: Integrate tag friends functionality in Edit method
    public class PostsController : Controller
    {
        private readonly SocialMediaDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private static int _groupId = 0;
        private static PostTagFriendsViewModel ViewModel = new PostTagFriendsViewModel();

        public PostsController(SocialMediaDbContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        #region Posts

        // GET: GroupPosts
        public async Task<IActionResult> GroupPosts()
        {
            var user = await this._userManager.GetUserAsync(User);

            TempData["groupPost"] = "true";
            if (TempData["groupId"] != null)
            {
                _groupId = int.Parse(TempData["groupId"].ToString());
            }

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
            var user = await this._userManager.GetUserAsync(User);

            TempData["userPost"] = "true";

            var posts = await _context.Posts
                .Where(a => a.AuthorId == user.Id && a.GroupId == null)
                .ToListAsync();

            return View(posts);
        }
        #endregion

        #region CRUD 

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
            var userId = this._userManager.GetUserId(User);
            var user = this._context.Users.FirstOrDefault(i => i.Id == userId);

            var viewModel = new PostTagFriendsViewModel()
            {
                CurrentUser = user,
                UserFriends = GetUserFriends(user)
            };
            ViewModel = viewModel;

            return View(viewModel);
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]PostTagFriendsViewModel viewModel)
        {
            var user = await this._userManager.GetUserAsync(User);
            ViewModel.CurrentUser = user;
            //Creates post in the current user`s profile
            if (ModelState.IsValid && TempData["userPost"].ToString() == "true")
            {
                var post = new Post()
                {
                    Author = user,
                    AuthorId = user.Id,
                    DatePosted = DateTime.Now,
                    Content = viewModel.Post.Content,
                    TaggedUsers = TagFriendsEntities()
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                ViewModel = null;
                return RedirectToAction(nameof(UserPosts));
            }
            //Creates post in a group
            else if (ModelState.IsValid && TempData["userGroup"].ToString() == "true")
            {
                var group = await this._context.Groups
                    .FirstOrDefaultAsync(i => i.GroupId == _groupId);

                if (group == null)
                {
                    return NotFound();
                }

                var post = new Post()
                {
                    Author = user,
                    AuthorId = user.Id,
                    DatePosted = DateTime.Now,
                    Content = viewModel.Post.Content,
                    GroupId = group.GroupId,
                    Group = group,
                    TaggedUsers = TagFriendsEntities()
                };

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                ViewModel = null;
                return RedirectToAction(nameof(GroupPosts));
            }

            return View(viewModel);
        }

        //GET: Posts/Edit/5
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

        //TODO: The DELETE statement conflicted with the REFERENCE constraint "TagFriendsToPost_FK". The conflict occurred in database "SocialMedia", table "dbo.TagFriends", column 'PostId'.
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
            var taggedFriends = _context.TagFriends
                .Where(i => i.PostId == id);
            
            //Removes all tagged friends in the post
            foreach (var taggedFriend in taggedFriends)
            {
                _context.TagFriends.Remove(taggedFriend);
            }
            
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserPosts));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        #endregion
        //TODO: Tag friends service : Entities
        private ICollection<TagFriends> TagFriendsEntities()
        {
            var userId = ViewModel.CurrentUser.Id;

            var tagFriendsEntities = new List<TagFriends>();
            foreach (var tagged in ViewModel.Tagged)
            {
                var tagFriendsEntity = new TagFriends()
                {
                    TaggerId = userId,
                    TaggedId = tagged.Id
                };
                tagFriendsEntities.Add(tagFriendsEntity);
            }
            return tagFriendsEntities;
        }

        //TODO: Tag friends service : TagFriend(string taggedId)
        public IActionResult TagFriend(string taggedId)
        {
            //Adds tagged user in tagged collection of the view model
            var taggedUser = this._context.Users
                .FirstOrDefault(i => i.Id == taggedId);

            ViewModel.Tagged.Add(taggedUser);

            //Remove tagged user from user friends list of view model
            //The concept is that if any of the tagged users exist in this collection, 
            //it does not make sense to be tagged twice in one post.
            var item = ViewModel.UserFriends.SingleOrDefault(i => i.Id == taggedUser.Id);
            ViewModel.UserFriends.Remove(item);

            return View("Create", ViewModel);
        }

        //TODO:Friendship service : GetUserFriends
        private List<User> GetUserFriends(User currentUser)
        {
            //Get current user friendships where it is addressee or requester
            var friendships = this._context.Friendships
                .Where(i => i.AddresseeId == currentUser.Id && i.Status == 1 ||
                            i.RequesterId == currentUser.Id && i.Status == 1)
                .ToList();

            var friends = new List<User>();
            //Add all friends
            foreach (var friendship in friendships)
            {
                var friend = new User();
                //If addressee is current user, add requester
                if (friendship.AddresseeId == currentUser.Id)
                {
                    friend = this._context.Users
                        .FirstOrDefault(i => i.Id == friendship.RequesterId);
                }
                else //If requester is current user, add addressee
                {
                    friend = this._context.Users
                        .FirstOrDefault(i => i.Id == friendship.AddresseeId);
                }
                friends.Add(friend);
            }

            return friends;
        }
    }

}
