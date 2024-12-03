using ImageSharingWithCloud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ImageSharingWithCloud.DAL
{
    public  class ApplicationDbInitializer
    {
        private const string InitAdminUser = "jfk@example.org";
        
        private readonly ApplicationDbContext _db;
        private readonly IImageStorage _imageStorage;
        private readonly ILogContext _logContext;
        private readonly ILogger<ApplicationDbInitializer> _logger;
        public ApplicationDbInitializer(ApplicationDbContext db, 
                                        IImageStorage imageStorage,
                                        ILogContext logContext,
                                        ILogger<ApplicationDbInitializer> logger)
        {
            _db = db;
            _imageStorage = imageStorage;   
            _logContext = logContext;
            _logger = logger;
        }

        public async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            if (! await IsEmptyDatabase(serviceProvider))
            {
                return;
            }
            /*
             * Initialize databases.
             */
            _logger.LogDebug("Clearing the database...");

            await _imageStorage.InitImageStorage();

            /*
             * Clear any existing data from the databases.
             */
            var images = await _imageStorage.GetAllImagesInfoAsync();
            foreach (var image in images)
            {
                await _imageStorage.RemoveImageAsync(image);
            }

            // _db.RemoveRange(_db.Users);
            await _db.SaveChangesAsync();

            _logger.LogDebug("Adding role: User");
            var idResult = await CreateRole(serviceProvider, "User");
            if (!idResult.Succeeded)
            {
                _logger.LogDebug("Failed to create User role!");
            }

            // TODO add other roles


            _logger.LogDebug("Adding user: jfk");
            idResult = await CreateAccount(serviceProvider, InitAdminUser, "Jfk@123", "Admin");
            if (!idResult.Succeeded)
            {
                _logger.LogDebug("Failed to create jfk user!");
            }
            

            _logger.LogDebug("Adding user: nixon");
            idResult = await CreateAccount(serviceProvider, "nixon@example.org", "nixon123", "User");
            if (!idResult.Succeeded)
            {
                _logger.LogDebug("Failed to create nixon user!");
            }
            
            
            _logger.LogDebug("Adding user: lbj");
            idResult = await CreateAccount(serviceProvider, "lbj@example.org", "lbj123", "User");
            if (!idResult.Succeeded)
            {
                _logger.LogDebug("Failed to create lbj user!");
            }

            _logger.LogDebug("Adding user: obama");
            idResult = await CreateAccount(serviceProvider, "obama@example.org", "obama123", "Supervisor");
            if (!idResult.Succeeded)
            {
                _logger.LogDebug("Failed to create obama user!");
            }

            // TODO add other users and assign more roles

            await _db.SaveChangesAsync();

        }

        private static async Task<bool> IsEmptyDatabase(IServiceProvider provider)
        {
            UserManager<ApplicationUser> userManager = provider
                .GetRequiredService
                    <UserManager<ApplicationUser>>();
            return await userManager.FindByNameAsync(InitAdminUser) == null;
        }

        private static async Task<IdentityResult> CreateRole(IServiceProvider provider,
                                                            string role)
        {
            RoleManager<IdentityRole> roleManager = provider
                .GetRequiredService
                       <RoleManager<IdentityRole>>();
            var idResult = IdentityResult.Success;
            if (await roleManager.FindByNameAsync(role) == null)
            {
                idResult = await roleManager.CreateAsync(new IdentityRole(role));
            }
            return idResult;
        }

        private static async Task<IdentityResult> CreateAccount(IServiceProvider provider,
                                                               string email, 
                                                               string password,
                                                               string role)
        {
            UserManager<ApplicationUser> userManager = provider
                .GetRequiredService
                       <UserManager<ApplicationUser>>();
            var idResult = IdentityResult.Success;

            if (await userManager.FindByNameAsync(email) == null)
            {
                var user = new ApplicationUser { UserName = email, Email = email };
                idResult = await userManager.CreateAsync(user, password);

                if (idResult.Succeeded)
                {
                    idResult = await userManager.AddToRoleAsync(user, role);
                }
            }

            return idResult;
        }

    }
}