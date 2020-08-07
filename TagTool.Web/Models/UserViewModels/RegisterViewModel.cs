using System.ComponentModel.DataAnnotations;
using TagTool.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TagTool.Web.Models
{
    public class RegisterViewModel
    {

        public SelectList Cities { get; set;}

        // Collect City Id and org Id in form
        [Required]
        public int CityID {get; set;}
        
        [Required]
        public string Name {get; set;}

        [Required]
        public string RoleString {get; set;}
        public Role GetRole()
        {
            if (RoleString == "Admin"){return Role.Admin;}
            else return Role.User;
        }
    
        [Required][EmailAddress]
        public string EmailAddress {get; set;}

        [Required]
        [StringLength(128, MinimumLength = 8, ErrorMessage= 
        "Password must be between 8 & 128 characters long.")]
        public string Password {get; set;}

        [Compare ("Password", ErrorMessage = "Confirm password matches")]
        public string PasswordConfirm {get; set;}
        
    }
}