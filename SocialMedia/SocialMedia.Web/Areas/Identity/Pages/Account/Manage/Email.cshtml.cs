using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SocialMedia.Data;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data.Models;

namespace SocialMedia.Web.Areas.Identity.Pages.Account.Manage
{
    public partial class EmailModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SocialMediaDbContext _context;

        public EmailModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            SocialMediaDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._context = context;
        }

        public string Email { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var email = await _userManager.GetEmailAsync(user);
            Email = email;

            Input = new InputModel
            {
                NewEmail = email,
            };

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (Input.NewEmail != email)
            {
                if (this._userManager.Users.Any(e => e.Email == Input.NewEmail))
                {
                    ModelState.AddModelError("Email", $"Email '{this.Input.NewEmail}' is already taken.");
                    await LoadAsync(user);
                    return Page();
                }

                var updateUser = await this._context.Users.FirstOrDefaultAsync(i =>i.Id == user.Id);
                updateUser.Email = this.Input.NewEmail;
                await this._context.SaveChangesAsync();

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your email is changed.";
                return RedirectToPage();
            }

            StatusMessage = "Your email is unchanged.";
            return RedirectToPage();
        }
    }
}
