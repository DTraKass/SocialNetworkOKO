using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialNetworkOKO.Models;

namespace SocialNetworkOKO.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Friend> Friends { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();

        }
    }
}
