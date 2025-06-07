using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PWS.Models;

namespace PWS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Whiskey> Whiskeys { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<TastingItem> TastingItems { get; set; }
        public DbSet<TastingResponse> TastingResponses { get; set; }
        public DbSet<Blog> Blogs { get; set; }
    }
}
