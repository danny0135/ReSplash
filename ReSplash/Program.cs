using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReSplash.Data;
using ReSplash.Models;
using ReSplash.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ReSplashContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReSplashContext") ?? throw new InvalidOperationException("Connection string 'ReSplashContext' not found.")));

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Logout";
        options.AccessDeniedPath = "/Users/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add uses authentication
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

//
// Photos API
//
app.MapGet("/photo/get/{id}", (int id, ReSplashContext _context) =>
{
    Photo? photo = _context.Photo
        .Include("User").Include("Category").Include("PhotoTags").Include("PhotoTags.Tag")
        .Where(m => m.PhotoId == id)
        .SingleOrDefault();
    if (photo != null)
    {
        ModalViewModel viewModal = new ModalViewModel();
        viewModal.PhotoId = photo.PhotoId;
        viewModal.FileName = photo.FileName;
        viewModal.PublishDate = photo.PublishDate;
        viewModal.Description = photo.Description;
        viewModal.CreatedBy = photo.User.Handle;
        viewModal.Category = photo.Category.CategoryName;
        viewModal.Tags = photo.PhotoTags.Select(t => t.Tag.TagName).ToList();

        return Results.Json(viewModal);
    }
    else
    {
        return Results.Text("Photo not found.");
    }
});



app.Run();