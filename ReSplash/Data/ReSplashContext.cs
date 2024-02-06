using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReSplash.Models;

namespace ReSplash.Data
{
    public class ReSplashContext : DbContext
    {
        public ReSplashContext (DbContextOptions<ReSplashContext> options)
            : base(options)
        {
        }

        public DbSet<ReSplash.Models.Photo> Photo { get; set; } = default!;

        public DbSet<ReSplash.Models.User> User { get; set; } = default!;

        public DbSet<ReSplash.Models.Category> Category { get; set; } = default!;

        public DbSet<ReSplash.Models.PhotoTag> PhotoTag { get; set; } = default!;

        public DbSet<ReSplash.Models.Tag> Tag { get; set; } = default!;
    }
}
