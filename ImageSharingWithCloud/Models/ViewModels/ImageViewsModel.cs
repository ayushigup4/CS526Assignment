using System.ComponentModel.DataAnnotations;

namespace ImageSharingWithCloud.Models.ViewModels
{
	public class ImageViewsModel
    {
        [Required]
        [Display(Name = "Only logs for today?")]
        public bool Today { get; init; }
    }
}

