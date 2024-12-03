using System.Diagnostics;
using ImageSharingWithCloud.DAL;
using ImageSharingWithCloud.Models;
using ImageSharingWithCloud.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ImageSharingWithCloud.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(UserManager<ApplicationUser> userManager, 
                              IImageStorage imageStorage, 
                              ApplicationDbContext db, 
                              ILogger<HomeController> logger)
            :base(userManager, imageStorage, db)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(String UserName = "Stranger")
        {
            CheckAda();
            ViewBag.Title = "Welcome!";
            var user = await GetLoggedInUser();
            if (user == null)
            {
                ViewBag.UserName = UserName;
            }
            else
            {
                ViewBag.UserName = user.UserName;
            }
            return View();
        }

        // public IActionResult Privacy()
        // {
        //     CheckAda();
        //     return View();
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string errId)
        {
            CheckAda();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrId = errId });
        }
    }
}
