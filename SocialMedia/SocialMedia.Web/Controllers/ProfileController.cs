using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Models;
using SocialMedia.Models.ViewModels;
using System;
using System.Threading.Tasks;

namespace SocialMedia.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly SocialMediaDbContext _context;
        private readonly UserManager<User> _userManager;

        
        public ProfileController(SocialMediaDbContext context, UserManager<User> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }
        
        
        private ProfileViewModel ViewModel { get; set; } = new ProfileViewModel();


        public async Task<IActionResult> IndexAsync()
        {
            var currentUser = await this._userManager.GetUserAsync(User);
            return View(currentUser);
        }
    }
}