using System.ComponentModel.DataAnnotations;
namespace TagTool.Web.Models
{
    public class EmailViewModel
    {
        [EmailAddress]
        public string FormerEmailAddress {get; set; }
        
        [EmailAddress][Required]
        public string NewEmailAddress {get; set; }
        public bool Notifications { get; set; }
    }
}