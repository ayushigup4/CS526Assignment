using System.ComponentModel.DataAnnotations;

namespace ImageSharingWithCloud.Models.ViewModels
{
    public class ImageView
        /*
         * View model for an image.
         */
    {
        [Required]
        [StringLength(40)]
        public String Caption {get; set;}

        [Required]
        [StringLength(200)]
        public String Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}",ApplyFormatInEditMode=true)]
        public DateTime DateTaken { get; set; }

        [ScaffoldColumn(false)]
        // Do not call this Id, it will confuse model binding when posting back to controller
        // because of the default route {controller}/{action}/{?Id}
        public string Id;

        [ScaffoldColumn(false)]
        // Link to blob storage (for logging).
        public string Uri;

        [ScaffoldColumn(false)]
        public String UserId { get; set; }

        [ScaffoldColumn(false)]
        public String UserName { get; set; }

        public IFormFile ImageFile { get; set; }

        public ImageView()
        {
        }
    }
}