using ImageSharingWithCloud.DAL;
using ImageSharingWithCloud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageSharingWithCloud.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext Db;

        protected readonly UserManager<ApplicationUser> UserManager;

        protected IImageStorage ImageStorage;

        protected BaseController(UserManager<ApplicationUser> userManager, 
                                 IImageStorage imageStorage,
                                 ApplicationDbContext db)
        {
            this.Db = db;
            this.UserManager = userManager;
            this.ImageStorage = imageStorage;
        }


        protected void CheckAda()
        {
            ViewBag.isADA = GetAdaFlag();
        }

        private bool GetAdaFlag()
        {
            var cookie = Request.Cookies["ADA"];
            return (cookie != null && "true".Equals(cookie));
        }

        protected async Task<ApplicationUser> GetLoggedInUser()
        {
            var user = HttpContext.User;
            if (user == null || user.Identity == null || user.Identity.Name == null)
            {
                return null;
            }
            return await UserManager.FindByNameAsync(user.Identity.Name);
        }

        protected ActionResult ForceLogin()
        {
            return RedirectToAction("Login", "Account");
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        protected IQueryable<Image> ApprovedImages(IQueryable<Image> images)
        {
            return images.Where(im => im.Valid && im.Approved);
        }


        protected IQueryable<ApplicationUser> ActiveUsers()
        {
            return UserManager.Users.Where(u => u.Active);
        }
    }
}