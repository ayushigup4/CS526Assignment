using Microsoft.AspNetCore.Identity;

namespace ImageSharingWithCloud.Models
{
    public sealed class ApplicationUser : IdentityUser
    {

        public bool Ada { get; set; }
        public bool Active { get; set; }

        public ApplicationUser()
        {
            Active = true;
            Ada = false;
        }

        public ApplicationUser(string u)
        {
            Active = true;
            UserName = u;
            Email = u;
            Ada = false;
        }

        public ApplicationUser(string u, bool isAda) 
        {
            Active = true;
            UserName = u;
            Email = u;
            Ada = isAda;
        }
    }
}
