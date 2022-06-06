using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Univer.Models;

namespace Univer.Data
{
    public class ApplicationDbContext : DbContext
    {
        private int userId;
        public User AuthorizedUser => Users.FirstOrDefault(x => x.Id == userId);
        public DbSet<User> Users { get; set; }
        public DbSet<University> Universities { get; set; }

        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<UniversityIndicator> UniversityIndicators { get; set; }
        public ApplicationDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            userId = int.Parse(httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "-1");
            
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(x => x.Role)
                .HasConversion<string>();
            modelBuilder.Entity<University>()
               .Property(x => x.Status)
               .HasConversion<string>();
        }
    }

}
