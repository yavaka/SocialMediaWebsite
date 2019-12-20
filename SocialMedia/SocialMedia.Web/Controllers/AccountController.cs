using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialMedia.Models;
using SocialMedia.Models.IdentityModels;

namespace SocialMedia.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IUserClaimsPrincipalFactory<User> _claimsPrincipalFactory;

        public AccountController(ILogger<AccountController> logger,
            UserManager<User> userManager,
            IUserClaimsPrincipalFactory<User> claimsPrincipalFactory)
        {
            this._logger = logger;
            this._userManager = userManager;
            this._claimsPrincipalFactory = claimsPrincipalFactory;
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


                    var result = await this._userManager.CreateAsync(user, model.Password);

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
        public IActionResult Login()
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
    }
}