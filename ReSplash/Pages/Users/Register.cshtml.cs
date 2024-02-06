using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Scripting;
using ReSplash.Data;
using ReSplash.Models;

namespace ReSplash.Pages.Users
{
    public class RegisterModel : PageModel
    {
        private readonly ReSplashContext _context;
        private readonly IConfiguration _configuration;

        [BindProperty]
        public User User { get; set; } = default!;

        [BindProperty]
        public string Token { get; set; } = string.Empty;

        // Constructor
        public RegisterModel(ReSplashContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.User == null || User == null)
            {
                return Page();
            }

            // Validate token
            if (Token != _configuration["Token"].ToString())
            {
                ModelState.AddModelError("Token", "Invalid token.");
                return Page();
            }


            // Encrypt password
            User.Password = BCrypt.Net.BCrypt.HashPassword(User.Password);

            _context.User.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Users/Login");
        }
    }
}