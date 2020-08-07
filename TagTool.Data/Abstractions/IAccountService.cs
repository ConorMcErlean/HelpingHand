using TagTool.Data.Models;
using System.Collections.Generic;

namespace TagTool.Data.Services
{
    /* 
    The abstraction of AccountSevice, allows the service to be injected through 
    dependency injection.
    */
    public interface IAccountService
    {
        /* A method to initialise the database. */
        public void Initialise();
        
        /* Create Methods */

            /* Method to register a new user. */
            User Register (string email, string Name, Role role, 
                string Password, City city);
            

        /* Get Methods */

            /* Method to get an account by email. */
            User GetAccountByEmail (string email);

            /* 
            Method to get a list of all user accounts, with an OrderBy  
            parameter to allow the list to be sorted by an attribute. 
            */
            IList<User> ViewAllAccounts(string OrderBy);

        /* Additional Account Methods */
            
            /* 
            Method to authenticate a user against the stored hashed password. 
            Takes parameters of the user email as a string, and the unhashed 
            password as a string.
            */
            User Authenticate (string email, string Password);

            /* Method to close/ delete a User account. */
            bool CloseAccount(User User); 

            /* Method to update a user */
            User UpdateUser(string Email, string NewEmail, bool Notifications);

            /* Method to update a password */
            bool UpdatePassword(string Email, string NewPassword);
    }

}