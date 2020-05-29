using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
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

            //Gets the post and its tag friend entities
            var post = await _context.Posts
                .Include(i => i.TaggedUsers)
                .Include(i => i.Author)
                .FirstOrDefaultAsync(m => m.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            ViewModel = new PostTagFriendsViewModel()
            {
                CurrentUser = user,
                Post = post,
                Tagged = GetTaggedFriends(post.PostId, user.Id)
            };


            //Pass current postId to CommentsController
            TempData["postId"] = id;

            return View(ViewModel);
        }

        // GET: Posts/Create
        public IActionResult Create(int? id)
        {
            var userId = this._userManager.GetUserId(User);
            var user = this._context.Users.FirstOrDefault(i => i.Id == userId);

            ViewModel = new PostTagFriendsViewModel()
            {
                CurrentUser = user,
                UserFriends = GetUserFriends(user)
            };
            
            //Assign the group id if it is not null
            if (id != null)
            {
                ViewModel.Post = new Post() 
                {
                    GroupId = id
                };
            }

            return View(ViewModel);
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]PostTagFriendsViewModel viewModel)
        {
            var user = await this._userManager.GetUserAsync(User);
            ViewModel.CurrentUser = user;
            //Creates post in the current user`s profile
            if (ModelState.IsValid)
            {
                var post = new Post()
                {
                    Author = user,
                    AuthorId = user.Id,
                    DatePosted = DateTime.Now,
                    Content = viewModel.Post.Content,
                    TaggedUsers = TagFriendEntities(),
                };
                
                if (viewModel.Post.GroupId != null)
                {
                    post.GroupId = viewModel.Post.GroupId;
                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                ViewModel = new PostTagFriendsViewModel();
                
                //Redirect to the given group details view
                if (viewModel.Post.GroupId != null)
                {
                    return RedirectToAction("Details","Groups",new { id = post.GroupId});
                }

                return RedirectToAction(nameof(UserPosts));
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

            var user = await this._userManager.GetUserAsync(User);

            ViewModel = new PostTagFriendsViewModel();
            ViewModel.CurrentUser = user;
            ViewModel.Post = post;
            ViewModel.Tagged = GetTaggedFriends(post.PostId, user.Id);
            /*If this method is invoked before GetTaggedFriends, 
              there will add all of the current user`s friends.
              Let`s get that user x is already tagged from the creation of the post.
              It does not make sense the current user to be allowed to tag user x twice.*/
            ViewModel.UserFriends = GetUserFriends(user);

            return View(ViewModel);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm]PostTagFriendsViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await this._userManager.GetUserAsync(User);
                    ViewModel.CurrentUser = user;
                    
                    var post = await this._context.Posts
                        .Include(i => i.TaggedUsers)
                        .FirstOrDefaultAsync(i => i.PostId == viewModel.Post.PostId);

                    post.Author = user;
                    post.AuthorId = user.Id;
                    post.Content = viewModel.Post.Content;

                    //Local tag friend entities
                    var tagFriendEntities = TagFriendEntities(post);
                    //Connected tag friend entities (In the db)
                    var postTagFriendEntities = post.TaggedUsers;
                    //If there is a mismatch between any record in the Local collection will be deleted from the Connected collection 
                    RemoveTaggedFriendRecords(postTagFriendEntities, tagFriendEntities);
                    //If there is a mismatch between any record in the Connected collection will be added from the Local collection 
                    AddLocalTaggedFriends(postTagFriendEntities, tagFriendEntities);
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(viewModel.Post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                ViewModel = new PostTagFriendsViewModel();
                return RedirectToAction(nameof(UserPosts));
            }

            return View(viewModel);
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
            var post = await _context.Posts
                .Include(i =>i.TaggedUsers)
                .Include(c =>c.Comments)
                .FirstOrDefaultAsync(i =>i.PostId == id);

            //Gets all comments tagged users
            var commentsTaggedUsers = GetCommentsTagFriendEntities(post.Comments);

            //Removes all tagged friends in this post and this post`s comments
            _context.TagFriends.RemoveRange(post.TaggedUsers.ToList());
            _context.TagFriends.RemoveRange(commentsTaggedUsers);
            _context.Posts.Remove(post);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(UserPosts));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        #endregion

        //TODO: Comments service: GetCommentsTaggedUsers(Collection of Comments)
        private ICollection<TagFriends> GetCommentsTagFriendEntities(ICollection<Comment> postComments)
        {
            var taggedUsers = new List<TagFriends>();
            foreach (var commentId in postComments.Select(i => i.Id))
            {
                //Gets the comment with tagged users collection
                var comment = this._context.Comments
                    .Include(i => i.TaggedUsers)
                    .FirstOrDefault(i => i.Id == commentId);

                taggedUsers.AddRange(comment.TaggedUsers);
            }
            return taggedUsers;
        }

        //TODO: Tag friends service : Entities(Post post)
        private ICollection<TagFriends> TagFriendEntities(Post post)
        {
            if (ViewModel.Tagged.Count == 0)
            {
                return new List<TagFriends>();
            }

            foreach (var tagged in ViewModel.Tagged)
            {
                if (!post.TaggedUsers.Any(i => i.TaggedId == tagged.Id &&
                                            i.TaggerId == ViewModel.CurrentUser.Id &&
                                            i.PostId == post.PostId))
                {
                    post.TaggedUsers.Add(new TagFriends()
                    {
                        TaggerId = ViewModel.CurrentUser.Id,
                        TaggedId = tagged.Id,
                    });
                }
            }
            //change it then
            return post.TaggedUsers;
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

        //TODO: Tag friends service : TagFriend(string taggedId, string taggerId)
        public IActionResult TagFriend(string taggedId, string viewName)
        {
            //Check is the last tagged friend is already been tagged,
            //if the tagged friends collection is not empty.
            if (ViewModel.Tagged.Count > 0)
            {
                if (ViewModel.Tagged.Last().Id == taggedId)
                {
                    return View(viewName, ViewModel);
                }
            }

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

        //TODO:Tag friends service : GetTaggedFriendsByPostIdAndUserId(int postId)
        private ICollection<User> GetTaggedFriends(int postId, string userId)
        {
            //TagFriend entities where users are tagged by the current user
            var tagFriendsEntities = this._context.TagFriends
                .Where(i => i.PostId == postId && i.TaggerId == userId)
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

        //Remove TagFriend entity records while edit the post
        private void RemoveTaggedFriendRecords(ICollection<TagFriends> postTaggedFriends, ICollection<TagFriends> tagFriendEntities)
        {
            //Compare already tagged friends collection in the post and
            //edited tag friends collection 
            var removedTagFriendEntities = postTaggedFriends.Except(tagFriendEntities)
                .ToList();

            this._context.TagFriends.RemoveRange(removedTagFriendEntities);
        }

        //Add TagFriend entities which are on local collection to the db 
        private void AddLocalTaggedFriends(ICollection<TagFriends> postTagFriendEntities, ICollection<TagFriends> tagFriendEntities)
        {
            var addedTagFriendEntities = tagFriendEntities.Except(postTagFriendEntities)
                .ToList();

            this._context.TagFriends.AddRange(addedTagFriendEntities);
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
    }

}
