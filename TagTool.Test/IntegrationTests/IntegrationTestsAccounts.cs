using TagTool.Web.Controllers;
using TagTool.Web.Models;
using TagTool.Data.Repositories;
using TagTool.Data.Services;
using TagTool.Data.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Moq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections;

namespace TagTool.Test
{
    [Collection("Integration")]
    public class IntegrationTestsAccounts
    {

        /*
        The following file contains integration tests related to User Accounts. 
        These tests may call on multiple components to complete the test. If 
        these tests fail, it indicates a system error may be occurring, when 
        working components are brought together.

        These tests will run automatically alongside the unit-tests.
        */

        // Controllers
        private readonly ReportController _ReportController;
        private readonly UserController _UserController;

        // Additional Dependencies
        private readonly ISeeder _Seeder;
        private readonly DataContext _db;
        private readonly HttpContext httpContext;

        public IntegrationTestsAccounts(
            IReportService _ReportService, 
            DataContext _db, 
            IAccountService _AccountService,
            ICityService _CityService,
            ISeeder Seeder)
            
            {
            // Controllers Set-Up
            _ReportController = new ReportController(_db, _ReportService);
            _UserController = new UserController(_db, _AccountService, _CityService);

            // Dependencies
            _Seeder = Seeder;
            this._db = _db;
            
            // Create Temp Data Disctionary So Alerts dont throw errors.
            httpContext = new DefaultHttpContext();
        }

        /* Below, tests that all the components required to properly register a 
        user are functional */
        [Fact]
        public void Integration_RegisterAUser_Success()
        {
            // Seed
             _Seeder.Seed();
            _UserController.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Call Register Page & Ensure it contains a select list
            var CreatePage = _UserController.Register() as ViewResult;
            var ViewModel = (RegisterViewModel) CreatePage.ViewData.Model;
            var SelectList = (IList) ViewModel.Cities.Items;
            // Check List is valid
            var ListOfCities = _db.Cities.ToList();
            Assert.Equal(ListOfCities, SelectList);

            // Fill ViewModel & Submit
            ViewModel.Name = "Sarah";
            ViewModel.EmailAddress = "Sarah@organisation.org";
            ViewModel.RoleString = "User";
            ViewModel.Password = "ThisAmazingPassword";
            ViewModel.CityID = _db.Cities.First().CityID;
            var Redirect = (RedirectToActionResult) 
            _UserController.Register(ViewModel);

            // Check Redirect
            Assert.Equal("AdminHome", Redirect.ActionName);
            Assert.Equal("Home", Redirect.ControllerName);

            // Check User is now in DB
            var createdUser = _db.Users.FirstOrDefault(c => 
            c.EmailAddress == ViewModel.EmailAddress); 
            Assert.NotNull(createdUser);
        }

        /* Below, tests that all the components required to view all user 
        accounts are functional */
        [Fact]
        public void Integration_ViewAllAccounts_Success()
        {
            // Seed
             _Seeder.Seed();

            // Call ViewAll Page & Ensure it contains a select list
            var ViewPage = _UserController.ViewAccounts("") as ViewResult;
            var ViewList = (IList) ViewPage.ViewData.Model;
            // Check List is valid
            var ListOfUsers = _db.Users.ToList();
            Assert.Equal(ListOfUsers, ViewList);

            // Try again with orderBy Name
            ViewPage = _UserController.ViewAccounts("Name") as ViewResult;
            ViewList = (IList) ViewPage.ViewData.Model;
            // Check List is valid
            ListOfUsers = _db.Users.OrderBy(u => u.Name).ToList();
            Assert.Equal(ListOfUsers, ViewList);

            // Try again with orderBy Email
            ViewPage = _UserController.ViewAccounts("Email") as ViewResult;
            ViewList = (IList) ViewPage.ViewData.Model;
            // Check List is valid
            ListOfUsers = _db.Users.OrderBy(u => u.EmailAddress).ToList();
            Assert.Equal(ListOfUsers, ViewList);

            // Try again with orderBy Role
            ViewPage = _UserController.ViewAccounts("Role") as ViewResult;
            ViewList = (IList) ViewPage.ViewData.Model;
            // Check List is valid
            ListOfUsers = _db.Users.OrderBy(u => u.Role).ToList();
            Assert.Equal(ListOfUsers, ViewList);

            // Try again with orderBy City
            ViewPage = _UserController.ViewAccounts("City") as ViewResult;
            ViewList = (IList) ViewPage.ViewData.Model;
            // Check List is valid
            ListOfUsers = _db.Users.Include(u => u.City).OrderBy(u => u.City).ToList();
            Assert.Equal(ListOfUsers, ViewList);
        }

        /* Below, tests that all the components required to close an account 
         are functional */
        [Fact]
        public void Integration_CloseAccount_Success()
        {
            // Seed
             _Seeder.Seed();
            _UserController.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Get all Accounts
            var ViewPage = _UserController.ViewAccounts("") as ViewResult;
            var ViewList = (IList) ViewPage.ViewData.Model;
            var Account = (User) ViewList[1];
            // Call Service
            var ConfirmPage =
             _UserController.CloseAccount(Account.EmailAddress) as ViewResult;
            

            // Check Return contains view model
            var ConfirmVM = (CloseUserViewModel) ConfirmPage.ViewData.Model;
            Assert.Equal(Account.EmailAddress, ConfirmVM.EmailAddress);
            Assert.Equal(Account.Name, ConfirmVM.Name);
            Assert.Equal(Account.UserID, ConfirmVM.UserID);
            Assert.Equal(Account.Role, ConfirmVM.Role);

            // Call Close Account to confirm
            var Redirect = (RedirectToActionResult) 
            _UserController.CloseAccount(ConfirmVM);

            // Check Redirect
            Assert.Equal("Index", Redirect.ActionName);
            Assert.Equal("Home", Redirect.ControllerName);

            // Check User is removed from DB
            var RemovedUser = _db.Users.FirstOrDefault
            (u => u.EmailAddress == ConfirmVM.EmailAddress);
            Assert.Null(RemovedUser);
        }

        /* Below, tests that all the components required to create a city are 
        functional */
        [Fact]
        public void Integration_CreateACity_Success()
        {
            // Seed
            _Seeder.Seed();
            _UserController.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Call View
            var CreatePage = _UserController.CreateCity();
            Assert.IsType<ViewResult>(CreatePage);

            // Fill View Model
            var ViewModel = new CityViewModel(){
                Name = "Test Case City"
            };

            // Call Controller
            var Redirect = (RedirectToActionResult) 
            _UserController.CreateCity(ViewModel);

            // Check Redirect
            Assert.Equal("Index", Redirect.ActionName);
            Assert.Equal("Home", Redirect.ControllerName);

            // Check City Added to DB
            var Created = _db.Cities.FirstOrDefault
            (c => c.Name == ViewModel.Name);
            Assert.NotNull(Created);
        }

        /* Below, tests that all the components required to access the user 
        settings menu are fully functional */
        [Fact]
        public void Integration_AccessUserSettings_Success()
        {
            // Seed
            _Seeder.Seed();

            // Open Settings Verify Page
            Assert.IsType<ViewResult>(_UserController.SettingsVerify());

            // Fill View Model
            var ViewModel = new LoginUserViewModel(){
                EmailAddress = "9h5an.test@inbox.testmail.app",
                Password ="ce@gIPP!Z!XESF2#b8sCaIKfi8maEHV@j7lLUfzegWX&2cBs&F&#"
            };

            // Call Controller
            var Redirect = (RedirectToActionResult) 
            _UserController.SettingsVerify(ViewModel);

            // Check Redirect
            var RouteValue = (Object[]) Redirect.RouteValues.Values;
            
            Assert.Equal("User", Redirect.ControllerName);
            Assert.Equal("UserSettings", Redirect.ActionName);
            Assert.Equal(ViewModel.EmailAddress, RouteValue[0].ToString());

            // Call This Action
            var SettingsPage = 
            _UserController.UserSettings(ViewModel.EmailAddress) as ViewResult;

            // Check Model contains Email
            var Email = (string) SettingsPage.ViewData.Model;
            Assert.Equal(ViewModel.EmailAddress, Email);
        }

        /* Below, tests that all the components required to change a user's 
        email are fully functional */
        [Fact]
        public void Integration_ChangeUserEmail_Success()
        {
            // Seed
            _Seeder.Seed();
            _UserController.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            // Known Account Email (With Notifications set to true)
            var AccountEmail = "9h5an.test@inbox.testmail.app";

            // Call Change Email
            var ChangeEmailPage = 
            _UserController.ChangeEmail(AccountEmail) as ViewResult;

            // Check Return contains ViewModel
            var ViewModel = (EmailViewModel) ChangeEmailPage.ViewData.Model;
            Assert.Equal(AccountEmail, ViewModel.FormerEmailAddress);
            Assert.Equal(AccountEmail, ViewModel.NewEmailAddress);
            Assert.True(ViewModel.Notifications);

            // Fill ViewModel with new Email and change notifications
            var NewEmail = "Rob@Orgs.co.uk";
            ViewModel.NewEmailAddress = NewEmail;
            ViewModel.Notifications = false;

            // Call Controller
            var Redirect = (RedirectToActionResult) 
            _UserController.ChangeEmail(ViewModel);

            // Check Redirect
            var RouteValue = (Object[]) Redirect.RouteValues.Values;
            
            Assert.Equal("User", Redirect.ControllerName);
            Assert.Equal("UserSettings", Redirect.ActionName);
            Assert.Equal(ViewModel.NewEmailAddress, RouteValue[0].ToString());

            // Check Account now has email
            var ModifiedUser = _db.Users.FirstOrDefault
            (u => u.EmailAddress == ViewModel.NewEmailAddress);
            Assert.NotNull(ModifiedUser);
        }

        /* Below, tests that all the components required to change a user's 
        password are fully functional */
        [Fact]
        public void Integration_ChangeUserPassword_Success()
        {
            // Seed
            _Seeder.Seed();
            _UserController.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            // Known Account Email
            var AccountEmail = "9h5an.test@inbox.testmail.app";
            // Get the hashed password for later checks
            var FormerPassHash = _db.Users.FirstOrDefault
            (u => u.EmailAddress == AccountEmail).HashedPassword;

            // Call Change Password
            var ChangePasswordPage = 
            _UserController.ChangePassword(AccountEmail) as ViewResult;

            // Check Return contains ViewModel
            var ViewModel = (PasswordViewModel) ChangePasswordPage.ViewData.Model;
            Assert.Equal(AccountEmail, ViewModel.EmailAddress);

            // Fill ViewModel with new Password
            var NewPassword = "ABCDEFGHIJK";
            ViewModel.NewPassword = NewPassword;

            // Call Controller
            var Redirect = (RedirectToActionResult) 
            _UserController.ChangePassword(ViewModel);

            // Check Redirect
            var RouteValue = (Object[]) Redirect.RouteValues.Values;
            
            Assert.Equal("User", Redirect.ControllerName);
            Assert.Equal("UserSettings", Redirect.ActionName);
            Assert.Equal(ViewModel.EmailAddress, RouteValue[0].ToString());

            // Check Account now has different password
            var ModifiedUserPass = _db.Users.FirstOrDefault
            (u => u.EmailAddress == AccountEmail).HashedPassword;
            Assert.NotEqual(FormerPassHash, ModifiedUserPass);
        }
    }
}
