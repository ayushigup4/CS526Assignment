using Microsoft.AspNetCore.Mvc.Rendering;

namespace ImageSharingWithCloud.Models.ViewModels
{
    public class ManageModel
    {
        public IList<SelectListItem> Users { get; set; }

    }
}
