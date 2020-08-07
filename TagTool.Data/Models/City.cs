using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TagTool.Data.Models
{
    public class City
    {
        public int CityID {get; set;}
        public string Name {get; set;}


        /* Entity Framework Relationship to Users here */
        public IList<User> UsersInCity { get; set;}

        /* Entity Framework Relationship to Users here */
        public IList<Report> ReportsInCity { get; set;}
    }
}