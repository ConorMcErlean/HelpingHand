using System.ComponentModel.DataAnnotations;
namespace TagTool.Web.Models
{
    public class PasswordViewModel
    {
        [EmailAddress]
        public string EmailAddress {get; set;}

        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage= 
        "Password must be between 8 & 128 characters long.")]
        public string NewPassword {get; set; }

        [Compare ("NewPassword", ErrorMessage = "Confirm password matches")]
        public string NewPasswordConfirm {get; set; }
    }
}