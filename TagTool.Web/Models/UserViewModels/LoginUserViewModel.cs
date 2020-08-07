using System.ComponentModel.DataAnnotations;


namespace TagTool.Web.Models
{
    public class LoginUserViewModel
    {

         [EmailAddress]
        public string EmailAddress {get; set;}

        public string Password {get; set;}
    }
}