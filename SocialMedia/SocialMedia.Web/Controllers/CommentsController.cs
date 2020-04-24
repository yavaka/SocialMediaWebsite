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

            var user = await this._userManager.GetUserAsync(User);

            ViewModel = new CommentTagFriendsViewModel();
            ViewModel.CurrentUser = user;
            ViewModel.Comment = comment;
            ViewModel.Tagged = GetTaggedFriends(comment.Id, user.Id);
            /*If this method is invoked before GetTaggedFriends, 
             there will add all of the current user`s friends.
             Let`s get that user x is already tagged from the creation of the comment.
             It does not make sense the current user to be allowed to tag user x twice.*/
            ViewModel.UserFriends = GetUserFriends(user);

            return View(ViewModel);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm]CommentTagFriendsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await this._userManager.GetUserAsync(User);
                    ViewModel.CurrentUser = user;
                    
                    var comment = await this._context.Comments
                        .Include(i =>i.TaggedUsers)
                        .FirstOrDefaultAsync(i => i.Id == viewModel.Comment.Id);

                    var post = await this._context.Posts
                        .FirstOrDefaultAsync(i => i.PostId == _postId);

                    comment.AuthorId = user.Id;
                    comment.Author = user;
                    comment.CommentedPostId = post.PostId;
                    comment.CommentedPost = post;
                    comment.Content = viewModel.Comment.Content;

                    //Local TagFriend entities
                    var tagFriendEntities = TagFriendEntities(comment);
                    //Connected TagFriend entities (In the db)
                    var commentTagFriendEntities = comment.TaggedUsers;
                    //If there is a mismatch between any record in the Local collection will be deleted from the Connected collection 
                    RemoveTaggedFriendRecords(commentTagFriendEntities, tagFriendEntities);
                    //If there is a mismatch between any record in the Connected collection will be added from the Local collection 
                    AddLocalTaggedFriends(commentTagFriendEntities, tagFriendEntities);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(viewModel.Comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewModel = new CommentTagFriendsViewModel();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
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
            var comment = await _context.Comments
                .Include(t =>t.TaggedUsers)
                .FirstOrDefaultAsync(i =>i.Id == id);
            
            //Removes all tagged friends
            _context.TagFriends.RemoveRange(comment.TaggedUsers);
            
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }

        #endregion

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

        //TODO:Tag friends service : RemoveTaggedFriendById(string id)
        public IActionResult RemoveTaggedFriend(string taggedId)
        {
            //Gets the tagged user
            var taggedUser = ViewModel.Tagged
                .FirstOrDefault(i => i.Id == taggedId);

            //Removes the tagged user 
            ViewModel.Tagged
                .Remove(taggedUser);

            //Adds already non tagged user in the UserFriends, 
            //so the current user can decide to tag it again in the same post.
            ViewModel.UserFriends.Add(taggedUser);

            return View("Edit", ViewModel);
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

        //TODO: Tag friends service : Entities(Comment comment)
        private ICollection<TagFriends> TagFriendEntities(Comment comment)
        {
            if (ViewModel.Tagged.Count == 0)
            {
                return new List<TagFriends>();
            }

            foreach (var tagged in ViewModel.Tagged)
            {
                if (!comment.TaggedUsers.Any(i => i.TaggedId == tagged.Id &&
                                            i.TaggerId == ViewModel.CurrentUser.Id &&
                                            i.CommentId == comment.Id))
                {
                    comment.TaggedUsers.Add(new TagFriends()
                    {
                        TaggerId = ViewModel.CurrentUser.Id,
                        TaggedId = tagged.Id,
                    });
                }
            }
            //change it then
            return comment.TaggedUsers;
        }

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

        //TODO:Tag friends service : GetTaggedFriendsByCommentIdAndUserId(int postId) 
        private ICollection<User> GetTaggedFriends(int commentId, string userId)
        {
            //TagFriend entities where users are tagged by the current user
            var tagFriendsEntities = this._context.TagFriends
                .Where(i => i.CommentId == commentId && i.TaggerId == userId)
                .Include(i => i.Tagged)
                .ToList();

            if (tagFriendsEntities != null)
            {
                return tagFriendsEntities
                    .Select(i => i.Tagged)
                    .ToList();
            }

            return null;
        }

        //Remove TagFriend entity records while edit the comment
        private void RemoveTaggedFriendRecords(ICollection<TagFriends> commentTaggedFriends, ICollection<TagFriends> tagFriendEntities)
        {
            //Compare already tagged friends collection in the comment and
            //edited tag friends collection 
            var removedTagFriendEntities = commentTaggedFriends.Except(tagFriendEntities)
                .ToList();

            this._context.TagFriends.RemoveRange(removedTagFriendEntities);
        }

        //Add TagFriend entities which are on local collection to the db 
        private void AddLocalTaggedFriends(ICollection<TagFriends> commentTagFriendEntities, ICollection<TagFriends> tagFriendEntities)
        {
            var addedTagFriendEntities = tagFriendEntities.Except(commentTagFriendEntities)
                .ToList();

            this._context.TagFriends.AddRange(addedTagFriendEntities);
        }
    }
}
