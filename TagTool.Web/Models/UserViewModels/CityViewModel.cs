using System.ComponentModel.DataAnnotations;

namespace TagTool.Web.Models
{
    public class CityViewModel
    {
        [Required]
        public string Name { get; set;}
    }
}