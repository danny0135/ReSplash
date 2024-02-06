using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReSplash.Data;
using ReSplash.Models;

namespace ReSplash.Pages.Photos
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ReSplash.Data.ReSplashContext _context;

        [BindProperty]
        public Photo Photo { get; set; } = default!;

        [BindProperty]
        public string strTags { get; set; } = default!;

        public List<SelectListItem> CategoryList { get; set; } = new();

        public EditModel(ReSplash.Data.ReSplashContext context)
        {
            _context = context;

            List<Category> _categories = _context.Category.ToList();
            foreach (Category category in _categories)
            {
                CategoryList.Add(new SelectListItem()
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName
                });
            }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Photo == null)
            {
                return NotFound();
            }

            var photo =  await _context.Photo.Include("Category").Include("PhotoTags").Include("PhotoTags.Tag").FirstOrDefaultAsync(m => m.PhotoId == id);

            if (photo == null)
            {
                return NotFound();
            }

            Photo = photo;

            foreach(PhotoTag photoTag in photo.PhotoTags)
            {
                if(!string.IsNullOrEmpty(strTags))
                {
                    strTags += ", ";
                }

                strTags += photoTag.Tag.TagName;
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // Add the User to the Photo
            int userId = int.Parse(User.Identity.Name);
            User? user = _context.User.Where(u => u.UserId == userId).SingleOrDefault();

            if (user != null)
            {
                Photo.User = user;
            }

            // Get and set the Category - get the category from the database and attach to this photo
            Category category = _context.Category.Single(m => m.CategoryId == Photo.Category.CategoryId);
            Photo.Category = category;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Photo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PhotoExists(Photo.PhotoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
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

            // Get the existing PhotoTags for this photo and delete them
            List<PhotoTag> existingPhotoTags = _context.PhotoTag.Where(a => a.PhotoId == Photo.PhotoId).ToList();
            foreach(PhotoTag photoTag in existingPhotoTags)
            {
                _context.PhotoTag.Remove(photoTag);
            }
            await _context.SaveChangesAsync();

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

        private bool PhotoExists(int id)
        {
          return (_context.Photo?.Any(e => e.PhotoId == id)).GetValueOrDefault();
        }
    }
}
