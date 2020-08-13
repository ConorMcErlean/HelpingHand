using System.ComponentModel.DataAnnotations;

namespace TagTool.Web.Models
{
    public class CityViewModel
    {
        [Required][StringLength(20, MinimumLength = 3, ErrorMessage =
            "City names must be between 3 to 20 charcters long.")]
        public string Name { get; set;}
    }
}