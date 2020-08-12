using System.ComponentModel.DataAnnotations;
namespace TagTool.Web.Models
{
    public class ReportViewModel
    {
        [Range(-90,90)][Required]
        public double Latitude { get; set;}
        [Range(-90, 90)][Required]
        public double Longitude { get; set;}
        public string AdditionalInfo { get; set;}
        public string GoogleAPIKey { get; set;}

    }
}