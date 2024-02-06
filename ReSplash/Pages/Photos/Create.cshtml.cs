using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReSplash.Data;
using ReSplash.Models;

namespace ReSplash.Pages.Photos
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ReSplashContext _context;
        IWebHostEnvironment _env;

        [BindProperty]
        public Photo Photo { get; set; } = default!;

        [BindProperty]
        public IFormFile ImageUpload { get; set; }

        [BindProperty]
        public string strTags { get; set; } = default!;

        public List<SelectListItem> CategoryList { get; set; } = new();

        public CreateModel(ReSplashContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;

            List<Category> categories = _context.Category.ToList();
            foreach(Category category in categories)
            {
                CategoryList.Add(new SelectListItem()
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName
                });
            }
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            //
            // Set default values
            //

            int userId = int.Parse(User.Identity.Name);
            User? user = _context.User.Where(u => u.UserId == userId).SingleOrDefault();

            if (user != null)
            {
                Photo.User = user;
            }

            // Make a unique image name
            string imageName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss-") + ImageUpload.FileName;

            Photo.FileName = imageName;
            Photo.PublishDate = DateTime.Now;
            Photo.ImageViews = 0;
            Photo.ImageDownloads = 0;

            // Get and set the Category - get the category from the database and attach to this photo
            Category category = _context.Category.Single(m => m.CategoryId == Photo.Category.CategoryId);
            Photo.Category = category;

            //
            // Validate and save Photo
            //
            if (!ModelState.IsValid || _context.Photo == null || Photo == null)
            {
                return Page();
            }

            // Save to database
            _context.Photo.Add(Photo);
            await _context.SaveChangesAsync();

            //
            // Upload the Image to the www/photos folder
            //

            string file = _env.ContentRootPath + "\\wwwroot\\photos\\" + imageName;

            using (FileStream fileStream = new FileStream(file, FileMode.Create))
            {
                ImageUpload.CopyTo(fileStream);
            }

            //
            // Add Tags for photo
            //

            // Split the user's input tags string into an array
            string[] userInputTags = strTags.Split(',');

            // Get all the tags from the database
            string[] existingTags = _context.Tag.Select(t => t.TagName).ToArray();

            // Keep a list of the tags that are new
            List<Tag> newPhotoTags = new();

            // Loop through the user's input tags
            foreach (string userInputTag in userInputTags)
            {
                // Trim the tag
                string trimmedTag = userInputTag.Trim();

                // If the tag is not in the database, add it to the newTags list
                if (!existingTags.Contains(trimmedTag))
                {
                    Tag newTag = new Tag() { TagName = trimmedTag };
                    newPhotoTags.Add(newTag);

                    // Add the new tag to the database
                    _context.Tag.Add(newTag);
                }
                else
                {
                    // If an existing tag, add it to the newTags list
                    Tag? newTag = _context.Tag.Where(t => t.TagName == trimmedTag).SingleOrDefault();
                    if (newTag != null)
                    {
                        newPhotoTags.Add(newTag);
                    }
                }
            }

            // Save the new tags to the database
            await _context.SaveChangesAsync();

            // Create a new PhotoTag for each new tag
            foreach (Tag newTag in newPhotoTags)
            {
                PhotoTag newPhotoTag = new PhotoTag() { Photo = Photo, Tag = newTag };
                _context.PhotoTag.Add(newPhotoTag);
            }
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}