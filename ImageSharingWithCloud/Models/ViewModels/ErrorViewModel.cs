namespace ImageSharingWithCloud.Models.ViewModels
{
    public class ErrorViewModel
    {
        public string RequestId { get; init; }

        public string ErrId { get; init; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}