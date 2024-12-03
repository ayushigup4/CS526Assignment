using Microsoft.EntityFrameworkCore;
using ImageSharingWithCloud.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ImageSharingWithCloud.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }

}