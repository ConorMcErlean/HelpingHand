using System.ComponentModel.DataAnnotations;
using TagTool.Data.Models;

namespace TagTool.Web.Models
{
    public class CloseUserViewModel
    {

        [EmailAddress]
        public string EmailAddress {get; set; }

        public int UserID {get; set;}
        public string Name {get; set;}
        /* The access level of user */
        public Role Role {get; set;}

    }
}