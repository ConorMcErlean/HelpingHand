using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TagTool.Data.Models
{
    public enum Role {Admin, User}
    public class User
    {
        public int UserID {get; set;}
        public string Name {get; set;}

        /* The access level of user */
        public Role Role {get; set;}

        /* Control of email notifications for user */
        public bool EmailNotifications {get; set;}
        
        [EmailAddress]
        public String EmailAddress {get; set;}

        public String HashedPassword {get; set;}


        /*  Entity Framework Relationship to City here */
        public int CityID { get; set;}
        public City City { get; set;}
    }
}