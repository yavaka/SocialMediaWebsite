using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                    
                    ViewBag.Message = $"{user.UserName} was registered!";

                    return RedirectToAction("Index","Home");
                }
            }
            return View();
        }
        #endregion
    }
}