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
    public class FriendshipsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SocialMediaDbContext _context;

        
        public FriendshipsController(UserManager<User> userManager,
            SocialMediaDbContext context)
        {
            this._userManager = userManager;
            this._context = context;
        }


        private static FriendshipViewModel ViewModel { get; set; } = new FriendshipViewModel();

        
        //Friends
        public async Task<IActionResult> Friends()
        {
            var user = await this._userManager.GetUserAsync(User);

            var friendsIds = this._context.Friendships
               .Where(a => a.AddresseeId == user.Id && a.Status == 1 ||
                      a.RequesterId == user.Id && a.Status == 1)
               .ToList();

            var friends = new List<User>();
            foreach (var friendId in friendsIds)
            {
                if (friendId.AddresseeId == user.Id)
                {
                    friends.Add(
                    this._context.Users
                    .FirstOrDefault(i => i.Id == friendId.RequesterId));
                }
                else if (friendId.RequesterId == user.Id)
                {
                    friends.Add(
                    this._context.Users
                    .FirstOrDefault(i => i.Id == friendId.AddresseeId));
                }
            }

            return View(friends);
        }

        //Non friends
        //Get Account/Users
        [HttpGet]
        public async Task<IActionResult> NonFriends()
        {
            //Get the current user
            var user = await this._userManager.GetUserAsync(User);

            //Get all other users except the current one
            var users = this._userManager.Users
                .Where(i => i.Id != user.Id)
                .ToList();

            var nonFriends = users;

            //All frienship of the current user
            var friendships = this._context.Friendships
                .Where(u => u.RequesterId == user.Id || u.AddresseeId == user.Id)
                .ToList();

            if (friendships.Count == 0)
            {
                return View(users);
            }
            else
            {
                //Check all of friendships
                foreach (var item in users.ToList())
                {
                    foreach (var friendship in friendships)
                    {
                        //If friendship exist user is removed
                        if (friendship.AddresseeId == item.Id ||
                            friendship.RequesterId == item.Id)
                        {
                            nonFriends.Remove(item);
                            break;
                        }
                    }
                }
            }

            return View(nonFriends);
        }

        //User profile
        //Get Account/UserProfile
        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            //Gets the url Account/UserProfile/UserId
            var reqUrl = Request.HttpContext.Request;
            //Convert the url into array
            var urlPath = reqUrl.Path
                .ToString()
                .Split('/')
                .ToArray();

            //Gets the current user
            var currentUser = await this._userManager.GetUserAsync(User);

            //Gets the user
            var user = await this._userManager.FindByIdAsync(urlPath[3]);

            //Check are the current user and other one are friends
            if (await this._context.Friendships.AnyAsync(
                i => i.AddresseeId == currentUser.Id && i.RequesterId == user.Id && i.Status == 1 ||
                i.AddresseeId == user.Id && i.RequesterId == currentUser.Id && i.Status == 1))
            {
                return View("UserProfile", user);
            }
            else
            {
                return View("NonFriendProfile", user);
            }
        }

        //Requester
        public async Task<IActionResult> SendRequest()
        {
            //Gets the url Account/SendRequest/UserId
            var reqUrl = Request.HttpContext.Request;
            //Convert the url into array
            var urlPath = reqUrl.Path
                .ToString()
                .Split('/')
                .ToArray();

            //Gets the user
            var addressee = await this._userManager.FindByIdAsync(urlPath[3]);
            var requester = await this._userManager.GetUserAsync(User);

            if (addressee == null || requester == null)
            {
                return NotFound();
            }
            else
            {
                //Add the addressee user in the pending collection
                ViewModel.Pending.Add(addressee);
            }

            var friendship = new Friendship()
            {
                AddresseeId = addressee.Id,
                Addressee = addressee,
                RequesterId = requester.Id,
                Requester = requester,
                Status = 0
            };

            await this._context.Friendships.AddAsync(friendship);
            await this._context.SaveChangesAsync();

            return RedirectToAction(nameof(FriendRequests));
        }

        //Addressee
        public async Task<IActionResult> FriendRequests()
        {
            var currentUser = await this._userManager.GetUserAsync(User);

            ViewModel.Requests = this._context.Friendships
                .Include(r => r.Requester)
                .Where(a => a.AddresseeId == currentUser.Id && a.Status == 0)
                .Select(r =>r.Requester)
                .ToList();

            ViewModel.Pending = this._context.Friendships
                .Include(a => a.Addressee)
                .Where(r => r.RequesterId == currentUser.Id && r.Status == 0)
                .Select(a => a.Addressee)
                .ToList();

            return View(ViewModel);
        }

        //Accept request
        public async Task<IActionResult> Accept()
        {
            //Gets the url Account/Accept/UserId
            var reqUrl = Request.HttpContext.Request;
            //Convert the url into array
            var urlPath = reqUrl.Path
                .ToString()
                .Split('/')
                .ToArray();

            var addressee = await this._userManager.GetUserAsync(User);

            var friendship = await this._context.Friendships
                .FirstOrDefaultAsync(i => i.RequesterId == urlPath[3] && i.AddresseeId == addressee.Id);

            friendship.Status = 1;

            await this._context.SaveChangesAsync();

            return RedirectToAction(nameof(FriendRequests));
        }

        //Reject request
        public async Task<IActionResult> Reject()
        {
            //Gets the url Account/Reject/UserId
            var reqUrl = Request.HttpContext.Request;
            //Convert the url into array
            var urlPath = reqUrl.Path
                .ToString()
                .Split('/')
                .ToArray();

            var addressee = await this._userManager.GetUserAsync(User);

            var friendship = await this._context.Friendships
                        .FirstOrDefaultAsync(i => i.RequesterId == urlPath[3] && i.AddresseeId == addressee.Id);

            this._context.Friendships.Remove(friendship);
            await this._context.SaveChangesAsync();

            return RedirectToAction(nameof(FriendRequests));
        }

    }
}