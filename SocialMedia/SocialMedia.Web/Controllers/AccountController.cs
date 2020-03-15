using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SocialMedia.Data;
using SocialMedia.Models;
using SocialMedia.Models.IdentityModels;

namespace SocialMedia.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;
        private readonly SignInManager<User> _signInManager;
        private readonly SocialMediaDbContext _context;

        public AccountController(ILogger<AccountController> logger,
            UserManager<User> userManager,
            IUserClaimsPrincipalFactory<User> claimsPrincipalFactory,
            SignInManager<User> signInManager,
            SocialMediaDbContext context)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._claimsPrincipalFactory = claimsPrincipalFactory;
            this._signInManager = signInManager;
            this._context = context;
        }

        //
        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            this._logger.LogInformation(Request.Path);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.FindByNameAsync(model.UserName);

                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = model.UserName,
                        Email = model.Email
                    };

                    await this._userManager.CreateAsync(user, model.Password);

                    this._logger.LogInformation($"{user.UserName} added successfully with Id {user.Id}", user);

                    TempData["Message"] = $"{user.UserName} was registered!";

                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        #endregion

        //
        #region Login
        [HttpGet]
        public IActionResult LoginAsync()
        {
            this._logger.LogInformation(Request.Path);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await this._userManager.FindByNameAsync(model.UserName);

                if (user != null && await this._userManager.CheckPasswordAsync(user, model.Password))
                {
                    var principal = await this._claimsPrincipalFactory.CreateAsync(user);

                    await HttpContext.SignInAsync("Identity.Application", principal);

                    this._logger.LogInformation($"{user.UserName} successfully signed in", user);

                    TempData["Message"] = $"{user.UserName} successfully signed in";
                    return RedirectToAction("Index", "Home");
                }
                this._logger.LogError("Invalid UserName or Password");
                ModelState.AddModelError("", "Invalid UserName or Password");
            }
            
            return View();
        }
        #endregion

        //Logout
        public async Task<IActionResult> LogoutAsync()
        {
            await this._signInManager.SignOutAsync();
            _logger.LogInformation($"User logged out.");

            TempData["Message"] = $"{User.Identity.Name} successfully logged out.";
            return RedirectToAction("Index", "Home");
        }

        //Edit user details
        [HttpGet]
        public IActionResult ManageUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ManageUser(User newUserDetails)
        {
            var user = await _userManager.GetUserAsync(User);

            //If there is no user throw exception
            if (user == null)
            {
                return NotFound(
                    $"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            user = UpdateUserDetails(user, newUserDetails);

            await this._userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            TempData["Message"] = "Your profile has been updated";

            return View();
        }

        private User UpdateUserDetails(User user, User newUserDetails)
        {
            //TODO: Check all props DOB, Gender, etc...
            //TODO: Separate user details to acc details and personal details
            //TODO: Show current details of the user
            if (user.Email != newUserDetails.Email)
                user.Email = newUserDetails.Email;
            if (user.FirstName != newUserDetails.FirstName)
                user.FirstName = newUserDetails.FirstName;
            if (user.LastName != newUserDetails.LastName)
                user.LastName = newUserDetails.LastName;
            if (user.Country != newUserDetails.Country)
                user.Country = newUserDetails.Country;

            return user;
        }

        #region Friendship
        
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
            //TODO: pass data to view ! issue - friend req.
            
            return View(nonFriends);
        }

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

            //Gets the user
            var user = await this._userManager.FindByIdAsync(urlPath[3]);

            if (user == null)
                return NotFound();

            return View(user);
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

            return RedirectToAction("NonFriends");
        }

        //Addressee
        public async Task<IActionResult> FriendRequests()
        {
            var addressee = await this._userManager.GetUserAsync(User);

            var requestersIds = this._context.Friendships
                .Where(a => a.AddresseeId == addressee.Id && a.Status == 0)
                .ToList();

            var requests = new List<User>();
            foreach (var requesterId in requestersIds)
            {
                requests.Add(
                    this._context.Users
                    .FirstOrDefault(i => i.Id == requesterId.RequesterId));
            }

            //TODO: pass data to view ! issue - pending
            
            return View(requests);
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

            return RedirectToAction("FriendRequests");
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

            return RedirectToAction("FriendRequests");
        }

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
        #endregion

    }
}