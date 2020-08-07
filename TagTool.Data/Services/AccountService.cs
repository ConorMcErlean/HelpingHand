using System.Linq;
using System.Collections.Generic;
using TagTool.Data.Models;
using TagTool.Data.Repositories;
using TagTool.Data.Security;
using Microsoft.EntityFrameworkCore;

namespace TagTool.Data.Services
{
    /* Concrete implementation of IAccountService */
    public class AccountService : IAccountService
    {
        /* Dependencies */
        private readonly DataContext _db;

        /* Constuctor with Constructor DI-injection */
        public AccountService(DataContext db)
        {
            _db = db;
        }

        /* Method that calls on Contexts initialise method */
        public void Initialise() { _db.Initialise(); }


        /* AccountService Create Methods */

        /* Method to create a new user */
        public User Register(string email, string Name, Role role,
            string Password, City city)
        {
            /* Check for pre-exisiting account */
            var Exists = GetAccountByEmail(email);
            if (Exists != null) { return null; }

            /* Otherwise create account */
            var User = new User
            {
                EmailAddress = email,
                Name = Name,
                Role = role,
                HashedPassword = Hasher.CalculateHash(Password),
                City = city
            };
            _db.Users.Add(User);
            _db.SaveChanges();
            return User;
        }// Register


        /* Account Service Get Methods */


        /* Method to get a user by email */
        public User GetAccountByEmail(string email)
        {
            var User = _db.Users.FirstOrDefault(u => u.EmailAddress == email);
            return User;
        }// GetAccountByEmail



        /* Method to get a list of User Accounts, with order option. */
        public IList<User> ViewAllAccounts(string OrderBy = null)
        {
            switch (OrderBy)
            {
                case "Name":
                    return _db.Users.Include(u => u.City).OrderBy(u => u.Name).ToList();
                case "Email":
                    return _db.Users.Include(u => u.City).OrderBy(u => u.EmailAddress).ToList();
                case "Role":
                    return _db.Users.Include(u => u.City).OrderBy(u => u.Role).ToList();
                case "City":
                    return _db.Users.Include(u => u.City).OrderBy(u => u.City).ToList();
                default:
                    return _db.Users.Include(u => u.City).OrderBy(u => u.UserID).ToList();
            }
        }


        /* Account Service other Methods */


        /* Method to authenticate a user login */
        public User Authenticate(string email, string Password)
        {
            var User = GetAccountByEmail(email);
            if (User == null) { return null; }
            if (Hasher.ValidateHash(User.HashedPassword, Password))
            {
                return User;
            }
            return null;
        }// Authenticate

        /* Method to delete an user account */
        public bool CloseAccount(User User)
        {
            if (User == null) { return false; }
            _db.Users.Remove(User);
            _db.SaveChanges();
            return true;
        }// CloseAccount

        /* Method to update a user */
        public User UpdateUser(string Email, string NewEmail, bool Notifications)
        {
            var User = GetAccountByEmail(Email);
            /* Values that may be updated */
            /* EmailAddress,  Notifications */
            User.EmailAddress = NewEmail;
            User.EmailNotifications = Notifications;
            _db.SaveChanges();
            return User;
        }
        /* Method to update a password */
        public bool UpdatePassword(string Email, string NewPassword)
        {
            var User = GetAccountByEmail(Email);
            User.HashedPassword = Hasher.CalculateHash(NewPassword);
            _db.SaveChanges();
            return true;
        }
    }

}