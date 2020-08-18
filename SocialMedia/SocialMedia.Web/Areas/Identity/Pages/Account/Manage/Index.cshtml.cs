namespace SocialMedia.Web.Areas.Identity.Pages.Account.Manage
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using SocialMedia.Data;
    using SocialMedia.Data.Models;

    public partial class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SocialMediaDbContext _context;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            SocialMediaDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._context = context;
        }

        //TODO: User/Profile picture
        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Display(Name = "Last name")]
            public string LastName { get; set; }

            //TODO: Date picker
            [Display(Name = "Date of birth")]
            [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
            public DateTime? DOB { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }

            [Display(Name = "Gender")]
            public Gender Gender { get; set; }

            [Display(Name = "Bio")]
            public string Bio { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                DOB = user.DOB,
                Bio = user.Bio,
                City = user.City,
                Country = user.Country
            };
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

        public async Task<IActionResult> OnPostAsync()
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

            //Update details
            if (Input.FirstName != user.FirstName)
            {
                try
                {
                    var updateUser = await this._context.Users.FirstOrDefaultAsync(i => i.Id == user.Id);
                    updateUser.FirstName = this.Input.FirstName;
                    await this._context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    StatusMessage = "Your first name has not been updated";
                    return RedirectToPage();
                }

            }
            if (Input.LastName != user.LastName)
            {
                try
                {
                    var updateUser = await this._context.Users.FirstOrDefaultAsync(i => i.Id == user.Id);
                    updateUser.LastName = this.Input.LastName;
                    await this._context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    StatusMessage = "Your last name has not been updated";
                    return RedirectToPage();
                }

            }
            if (Input.DOB != user.DOB)
            {
                try
                {
                    var updateUser = await this._context.Users.FirstOrDefaultAsync(i => i.Id == user.Id);
                    updateUser.DOB = Input.DOB;
                    await this._context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    StatusMessage = "Your date of birth has not been updated";
                    return RedirectToPage();
                }

            }
            if (Input.Gender != user.Gender)
            {
                try
                {
                    var updateUser = await this._context.Users.FirstOrDefaultAsync(i => i.Id == user.Id);
                    updateUser.Gender = Input.Gender;
                    await this._context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    StatusMessage = "Your gender has not been updated";
                    return RedirectToPage();
                }

            }
            if (Input.Bio != user.Bio)
            {
                try
                {
                    var updateUser = await this._context.Users.FirstOrDefaultAsync(i => i.Id == user.Id);
                    updateUser.Bio = Input.Bio;
                    await this._context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    StatusMessage = "Your bio has not been updated";
                    return RedirectToPage();
                }

            }
            if (Input.Country != user.Country)
            {
                try
                {
                    var updateUser = await this._context.Users.FirstOrDefaultAsync(i => i.Id == user.Id);
                    updateUser.Country = Input.Country;
                    await this._context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    StatusMessage = "Your country has not been updated";
                    return RedirectToPage();
                }

            }
            if (Input.City != user.City)
            {
                try
                {
                    var updateUser = await this._context.Users.FirstOrDefaultAsync(i => i.Id == user.Id);
                    updateUser.City = Input.City;
                    await this._context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    StatusMessage = "Your city has not been updated";
                    return RedirectToPage();
                }

            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
