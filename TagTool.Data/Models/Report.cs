using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TagTool.Data.Models
{
   /* Model for manipulation of Reports */ 
    public class Report
    {
        
        public int ReportID { get; set;}
        [Range(-90,90)]
        public double Latitude { get; set;}
        [Range(-90, 90)]
        public double Longitude { get; set;}
        public DateTime CreatedAt { get; set;}
        public string AdditionalInfo { get; set;}
        public Boolean Active {get; set;}

        public string ThreeWordAddress { get; set;}

        /*  Entity Framework Relationship to City here */
        public int CityID { get; set;}
        public City City { get; set;}

        public string CreatedAtPretty()
        {
            var Created = DateTime.Now - CreatedAt;
            if (Created > new TimeSpan(2, 0, 0))
            {
                return Created.Hours + " Hours ago";
            }
            else if (Created > new TimeSpan(1, 0, 0))
            {
                return Created.Hours + " Hour ago";
            }
            else if (Created > new TimeSpan(0, 1, 0))
            {
                return Created.Minutes + " Minutes ago";
            }
            else
            {
                return Created.Seconds + " Seconds ago";
            }
        }

        public string ActivePretty()
        {
            if (Active)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }
    }
}