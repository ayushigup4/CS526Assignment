using ImageSharingWithCloud.Models;

namespace ImageSharingWithCloud.DAL
{
    public interface IImageStorage
    {
        public Task InitImageStorage();

        public Task<string> SaveImageInfoAsync(Image image);

        public Task<Image> GetImageInfoAsync(string userId, string imageId);

        public Task<IList<Image>> GetAllImagesInfoAsync();

        public Task<IList<Image>> GetImageInfoByUserAsync(ApplicationUser user);

        public Task UpdateImageInfoAsync(Image image);

        public Task SaveImageFileAsync(IFormFile imageFile, string userId, string imageId);

        public Task RemoveImageAsync(Image image);

        public Task RemoveImagesAsync(ApplicationUser user);

        public string ImageUri(string userId, string imageId);
    }
}
