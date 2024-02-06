using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ReSplash.Data;
using ReSplash.Models;
using System.Security.Claims;

namespace ReSplash.Pages.Users
{
    public class LoginModel : PageModel
    {
        private readonly ReSplashContext _context;

        [BindProperty]
        public User User { get; set; } = default!;

        // Constructor
        public LoginModel(ReSplashContext context)
        {
            _context = context;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.User == null || User == null)
            {
                return Page();
            }

            // Get the user from the database
            var dbUser = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Email);

            // If the user doesn't exist, return to the login page
            if (dbUser == null)
            {
                ModelState.AddModelError("User.Email", "Username not found.");
                return Page();
            }

            // Check the password
            if (!BCrypt.Net.BCrypt.Verify(User.Password, dbUser.Password))
            {
                ModelState.AddModelError("User.Password", "Incorrect password.");
                return Page();
            }

            // Setup session
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, dbUser.UserId.ToString()),
                new Claim("FullName", dbUser.Name),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties());

            return RedirectToPage("/Photos/Index");
        }
    }
}