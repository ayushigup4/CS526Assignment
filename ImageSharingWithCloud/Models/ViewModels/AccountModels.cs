using System.ComponentModel.DataAnnotations;

namespace ImageSharingWithCloud.Models.ViewModels
{

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; init; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; init; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; init; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; init; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; init; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; init; }
    }

    public class LogoutModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; init; }
    }

    public class AccessDeniedModel
    {
        [Required]
        [Display(Name = "Return Url")]
        public string ReturnUrl { get; init; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; init; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; init; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; init; }

        [Required]
        [Display(Name = "Are you visually impaired?")]
        public string Ada { get; init; }
    }

}
