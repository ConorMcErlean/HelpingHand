using TagTool.Web.Controllers;
using TagTool.Web.Models;
using TagTool.Data.Repositories;
using TagTool.Data.Services;
using TagTool.Data.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace TagTool.Test
{
    public class UserControllerTest
    {

        /*
        The following file contains all unit tests on the User Controller
        in TagTool.Web.Controllers.

        Note: Login & Logout are not tested, due to my inabilitiy to properly
        mock the HttpContext for this function.
        */
        
        private readonly UserController _Controller;
        private readonly Mock<IAccountService> _svc;
        private readonly Mock<ICityService> _Csvc;
        private readonly DataContext _db;
        private readonly HttpContext httpContext;

        List<City> Cities = new List<City>()
        {
            new City(){Name = "City1", CityID = 1},
            new City(){Name = "City2", CityID = 2}
        };

        public UserControllerTest(){

            _svc = new Mock<IAccountService>();
            _Csvc = new Mock<ICityService>();
            _Csvc.Setup(c => c.GetCities()).Returns(Cities);
            _db = new TestContext().GetContext("UserControllerTest");
            _Controller = new UserController(_db, _svc.Object, _Csvc.Object);
            // Create Temp Data Disctionary So Alerts dont throw errors.
            httpContext = new DefaultHttpContext();
        }

        [Fact]
        public void UserController_Register_ReturnsViewWithSelectList()
        {
            // When
            var Page = _Controller.Register() as ViewResult;
            var vm = (RegisterViewModel) Page.ViewData.Model;
            var SelectListData = (IList) vm.Cities.Items;
    
            // Then
            Assert.Equal(Cities, SelectListData); 
        }

        [Fact]
        public void UserController_Register_InvalidModel_ReturnsViewWithSelectList()
        {
            // Given
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var vm = new RegisterViewModel()
            {
                Cities = new SelectList(Cities, "CityID", "Name")
            };
            _Controller.ModelState.AddModelError("Latitude", "Latitude is Required");
            
            // When
            var Page = _Controller.Register(vm) as ViewResult;

            var ReturnedVm = (RegisterViewModel) Page.ViewData.Model;
            var SelectListData = (IList) ReturnedVm.Cities.Items;
    
            // Then
            Assert.Equal(Cities, SelectListData); 
        }

        [Fact]
        public void UserController_Register_InvalidModel_DoesNotCallService()
        {
            // Given
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var vm = new RegisterViewModel()
            {
                Cities = new SelectList(Cities, "CityID", "Name")
            };
            _Controller.ModelState.AddModelError("Latitude", "Latitude is Required");
            _svc.Setup(
                m => m.Register(
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<Role>(), 
                    It.IsAny<String>(),
                    It.IsAny<City>()
                    )).Returns(new User());
            // When
            var Page = _Controller.Register(vm) as ViewResult;
    
            // Then
            _svc.Verify(r => r.Register(
                It.IsAny<String>(),
                It.IsAny<String>(),
                It.IsAny<Role>(), 
                It.IsAny<String>(),
                It.IsAny<City>()), Times.Never());
        }

        [Fact]
        public void UserController_Register_ValidModel_CallsService()
        {
            // Given
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var vm = new RegisterViewModel()
            {
                Cities = new SelectList(Cities, "CityID", "Name")
            };
            _svc.Setup(
                m => m.Register(
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<Role>(), 
                    It.IsAny<String>(),
                    It.IsAny<City>()
                    )).Returns(new User());
            // When
            var Page = _Controller.Register(vm) as ViewResult;
    
            // Then
            _svc.Verify(r => r.Register(
                It.IsAny<String>(),
                It.IsAny<String>(),
                It.IsAny<Role>(), 
                It.IsAny<String>(),
                It.IsAny<City>()), Times.Once());
        }

        [Fact]
        public void UserController_Register_ValidModel_RedirectsIfUserCreated()
        {
            // Given
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var vm = new RegisterViewModel()
            {
                Cities = new SelectList(Cities, "CityID", "Name")
            };
            _svc.Setup(
                m => m.Register(
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<Role>(), 
                    It.IsAny<String>(),
                    It.IsAny<City>()
                    )).Returns(new User());
            // When
            var Result = (RedirectToActionResult) _Controller.Register(vm);
            
            // Then 
            Assert.Equal("AdminHome", Result.ActionName);
            Assert.Equal("Home", Result.ControllerName);
        }

        [Fact]
        public void UserController_Register_ValidModel_RedirectsIfUserExists()
        {
            // Given
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var vm = new RegisterViewModel()
            {
                Cities = new SelectList(Cities, "CityID", "Name")
            };
            User u = null;
            _svc.Setup(
                m => m.Register(
                    It.IsAny<String>(),
                    It.IsAny<String>(),
                    It.IsAny<Role>(), 
                    It.IsAny<String>(),
                    It.IsAny<City>()
                    )).Returns(u);
            // When
            var Page = _Controller.Register(vm) as ViewResult;

            var ReturnedVm = (RegisterViewModel) Page.ViewData.Model;
            var SelectListData = (IList) ReturnedVm.Cities.Items;
    
            // Then
            Assert.Equal(Cities, SelectListData); 
        }

        [Fact]
        public void UserController_Login_ReturnsView()
        {
            // When
            var Page = _Controller.Login();

            // Then
            Assert.IsType<ViewResult>(Page); 
        }

        [Fact]
        public async void UserController_Login_CallsAuthService()
        {
            // Given
            var LVM = new LoginUserViewModel()
            {
                EmailAddress = "X", 
                Password = "Y"
            };
            User nullReturn = null;
            _svc.Setup(
                m => m.Authenticate(It.IsAny<String>(),It.IsAny<String>()
                    )).Returns(nullReturn);
            // When
            var Page = await _Controller.Login(LVM);
    
            // Then
            _svc.Verify(r => r.Authenticate( 
                It.IsAny<String>(), 
                It.IsAny<String>()), Times.Once());
        }

        [Fact]
        public async void UserController_Login_NonAuthed_ReturnsViewModel()
        {
            // Given
            var LVM = new LoginUserViewModel()
            {
                EmailAddress = "X", 
                Password = "Y"
            };
            User nullReturn = null;
            _svc.Setup(
                m => m.Authenticate(It.IsAny<String>(),It.IsAny<String>()
                    )).Returns(nullReturn);
            // When
            var Page = await _Controller.Login(LVM) as ViewResult;
    
            // Then
            Assert.Equal(LVM, Page.ViewData.Model); 
        }

        [Fact]
        public void UserController_ViewAccounts_CallsService()
        {
            // Given
            List<User> UserList1 = new List<User>{
                new User(){UserID = 1},
                new User(){UserID = 2}
            };
            List<User> UserList2 = new List<User>{
                new User(){UserID = 2},
                new User(){UserID = 1}
            };

            _svc.Setup(
                m => m.ViewAllAccounts("OrderBy1")).Returns(UserList1);
            _svc.Setup(
                m => m.ViewAllAccounts("OrderBy2")).Returns(UserList2);
            // When
            var Page = _Controller.ViewAccounts("OrderBy1") as ViewResult;
    
            // Then
           _svc.Verify(r => r.ViewAllAccounts("OrderBy1"), Times.Once());
           _svc.Verify(r => r.ViewAllAccounts("OrderBy2"), Times.Never());
        }

        [Fact]
        public void UserController_ViewAccounts_ResturnsList()
        {
            // Given
            List<User> UserList1 = new List<User>{
                new User(){UserID = 1},
                new User(){UserID = 2}
            };
            List<User> UserList2 = new List<User>{
                new User(){UserID = 2},
                new User(){UserID = 1}
            };

            _svc.Setup(
                m => m.ViewAllAccounts("OrderBy1")).Returns(UserList1);
            _svc.Setup(
                m => m.ViewAllAccounts("OrderBy2")).Returns(UserList2);
            // When
            var Page = _Controller.ViewAccounts("OrderBy1") as ViewResult;
    
            // Then
            Assert.Equal(UserList1, Page.ViewData.Model);
        }

        [Fact]
        public void UserController_ViewAccounts_DifferentOrderBy_ResturnsList()
        {
            // Given
            List<User> UserList1 = new List<User>{
                new User(){UserID = 1},
                new User(){UserID = 2}
            };
            List<User> UserList2 = new List<User>{
                new User(){UserID = 2},
                new User(){UserID = 1}
            };

            _svc.Setup(
                m => m.ViewAllAccounts("OrderBy1")).Returns(UserList1);
            _svc.Setup(
                m => m.ViewAllAccounts("OrderBy2")).Returns(UserList2);
            // When
            var Page = _Controller.ViewAccounts("OrderBy2") as ViewResult;
    
            // Then
            Assert.Equal(UserList2, Page.ViewData.Model);
        }

        [Fact]
        public void UserController_CloseAccount_ValidEmail_ReturnsViewModel()
        {
            // Given
            var User = new User()
            {
                EmailAddress = "XX@XX.XX",
                Name = "XX",
                UserID = 1,
                Role = Role.User
            };
            _svc.Setup(
                m => m.GetAccountByEmail("XX@XX.XX")).Returns(User);
            // When
            var Page = _Controller.CloseAccount("XX@XX.XX") as ViewResult;
            var vm = (CloseUserViewModel) Page.ViewData.Model;

            // Then
            Assert.Equal(User.EmailAddress, vm.EmailAddress);
            Assert.Equal(User.Name, vm.Name);
            Assert.Equal(User.UserID, vm.UserID);
            Assert.Equal(User.Role, vm.Role);
        }

        [Fact]
        public void UserController_CloseAccount_InValidEmail_ReturnsNotFound()
        {
            // Given
        
            User User = null;
            _svc.Setup(
                m => m.GetAccountByEmail("XX@XX.XX")).Returns(User);

            // When
            IActionResult Page = _Controller.CloseAccount("XX@XX.XX");
            
            // Then
            Assert.IsType<NotFoundResult>(Page);
        }

        [Fact]
        public void UserController_CloseAccount_PassedUSer_Redirects()
        {
            // Given
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var User = new User()
            {
                EmailAddress = "XX@XX.XX",
                Name = "XX",
                UserID = 1,
                Role = Role.User
            };
            var UserVM = new CloseUserViewModel()
            {
                EmailAddress = "XX@XX.XX",
                Name = "XX",
                UserID = 1,
                Role = Role.User
            };
            _svc.Setup(
                m => m.GetAccountByEmail("XX@XX.XX")).Returns(User);
            _svc.Setup(m => m.CloseAccount(User)).Returns(true);

            // When
            var Result = (RedirectToActionResult) _Controller.CloseAccount(UserVM);
            
            // Then 
            Assert.Equal("Index", Result.ActionName);
            Assert.Equal("Home", Result.ControllerName);
        }

        [Fact]
        public void UserController_CloseAccount_PassedUser_CallsService()
        {
            // Given
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var User = new User()
            {
                EmailAddress = "XX@XX.XX",
                Name = "XX",
                UserID = 1,
                Role = Role.User
            };
            var UserVM = new CloseUserViewModel()
            {
                EmailAddress = "XX@XX.XX",
                Name = "XX",
                UserID = 1,
                Role = Role.User
            };
            _svc.Setup(
                m => m.GetAccountByEmail("XX@XX.XX")).Returns(User);
            _svc.Setup(m => m.CloseAccount(User)).Returns(true);
            
            // When
            var Result =  _Controller.CloseAccount(UserVM);
            
            // Then 
            _svc.Verify(r => r.CloseAccount(User), Times.Once());
        }

        [Fact]
        public void UserController_CreateCity_ReturnsView()
        {
            // When
            var Page = _Controller.CreateCity();

            // Then
            Assert.IsType<ViewResult>(Page); 
        }

        [Fact]
        public void UserController_CreateCity_InvalidModel_DoesNotCallService()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _Controller.ModelState.AddModelError("Name", "Name Error");
            var CityVM = new CityViewModel()
            {
                Name = "City"
            };
            _Csvc.Setup(
                m => m.CreateCity(CityVM.Name)).Returns(new City());
            
            // When
            var Result =  _Controller.CreateCity(CityVM);
            
            // Then 
            _Csvc.Verify(r => r.CreateCity(CityVM.Name), Times.Never());
        }

        [Fact]
        public void UserController_CreateCity_InvalidModel_ReturnsVM()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _Controller.ModelState.AddModelError("Name", "Name Error");
            var CityVM = new CityViewModel()
            {
                Name = "City"
            };
            _Csvc.Setup(
                m => m.CreateCity(CityVM.Name)).Returns(new City());
            
            // When
            var Result =  _Controller.CreateCity(CityVM) as ViewResult;
            
            // Then 
            Assert.Equal(CityVM, Result.ViewData.Model);
        }

        [Fact]
        public void UserController_CreateCity_ValidModel_CallsService()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            
            var CityVM = new CityViewModel()
            {
                Name = "City"
            };
            _Csvc.Setup(
                m => m.CreateCity(CityVM.Name)).Returns(new City());
            
            // When
            var Result =  _Controller.CreateCity(CityVM);
            
            // Then 
            _Csvc.Verify(r => r.CreateCity(CityVM.Name), Times.Once());
        }

        [Fact]
        public void UserController_CreateCity_ValidModel_New_Redirects()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            
            var CityVM = new CityViewModel()
            {
                Name = "City"
            };
            _Csvc.Setup(
                m => m.CreateCity(CityVM.Name)).Returns(new City());
            
            // When
            var Result = (RedirectToActionResult) _Controller.CreateCity(CityVM);
            
            // Then 
            Assert.Equal("Home", Result.ControllerName);
            Assert.Equal("Index", Result.ActionName);
        }

        [Fact]
        public void UserController_CreateCity_ValidModel_AlreadyExists_RetrurnsVM()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            
            var CityVM = new CityViewModel()
            {
                Name = "City"
            };
            City nullCity = null;
            _Csvc.Setup(
                m => m.CreateCity(CityVM.Name)).Returns(nullCity);
            
            // When
            var Result = _Controller.CreateCity(CityVM) as ViewResult;
            
            // Then 
            Assert.Equal(CityVM, Result.ViewData.Model);
        }

        [Fact]
        public void UserController_UserSettings_EmailShouldPass()
        {
            // Given
            string Email = "XX@XX.XX";
            
            // When
            var Page = _Controller.UserSettings(Email) as ViewResult;

            // Then
            Assert.Equal(Email, Page.ViewData.Model);
        }

        [Fact]
        public void UserController_ChangeEmail_InvalidEmail_ReturnsNotFound()
        {
            // Given
            string Email = "XX@XX.XX";

            User NullUser = null;
            _svc.Setup(
                m => m.GetAccountByEmail(Email)).Returns(NullUser);
            
            // When
            IActionResult Result =  _Controller.ChangeEmail(Email);
            
            // Then 
            Assert.IsType<NotFoundResult>(Result);
        }

        [Fact]
        public void UserController_ChangeEmail_ValidEmail_ReturnsViewModel()
        {
            // Given
            string Email = "XX@XX.XX";

            User User = new User(){
                EmailAddress = "xx@xx.xx",
                EmailNotifications = false
            };
            _svc.Setup(
                m => m.GetAccountByEmail(Email)).Returns(User);
            
            // When
            var Result =  _Controller.ChangeEmail(Email) as ViewResult;
            var Model = (EmailViewModel) Result.ViewData.Model;
            
            // Then 
            Assert.Equal(User.EmailAddress, Model.NewEmailAddress);
            Assert.Equal(User.EmailAddress, Model.FormerEmailAddress);
            Assert.Equal(User.EmailNotifications, Model.Notifications);
        }

        [Fact]
        public void UserController_ChangeEmail_InValidViewModel_ReturnsViewModel()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new EmailViewModel(){
                FormerEmailAddress = "xx@xx.xx",
                NewEmailAddress = "xx@xx.xx",
                Notifications = false
            };
            _svc.Setup(
                m => m.UpdateUser(
                    UserVm.FormerEmailAddress,
                    UserVm.NewEmailAddress, 
                    UserVm.Notifications)).Returns(new User());
            _Controller.ModelState.AddModelError("Oh Oh", "Oh, oh");

            // When
            var Result =  _Controller.ChangeEmail(UserVm) as ViewResult;
            var Model = (EmailViewModel) Result.ViewData.Model;
            
            // Then 
            Assert.Equal(UserVm, Model);
        }

        [Fact]
        public void UserController_ChangeEmail_InValidViewModel_DoesNotCallService()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new EmailViewModel(){
                FormerEmailAddress = "xx@xx.xx",
                NewEmailAddress = "xx@xx.xx",
                Notifications = false
            };
            _svc.Setup(
                m => m.UpdateUser(
                    UserVm.FormerEmailAddress,
                    UserVm.NewEmailAddress, 
                    UserVm.Notifications)).Returns(new User());
            _Controller.ModelState.AddModelError("Oh Oh", "Oh, oh");

            // When
            var Result =  _Controller.ChangeEmail(UserVm) as ViewResult;
            var Model = (EmailViewModel) Result.ViewData.Model;
            
            // Then 
            _svc.Verify(r => r.UpdateUser(
                    UserVm.FormerEmailAddress,
                    UserVm.NewEmailAddress, 
                    UserVm.Notifications), Times.Never());
        }

        [Fact]
        public void UserController_ChangeEmail_ValidViewModel_CallsService()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new EmailViewModel(){
                FormerEmailAddress = "xx@xx.xx",
                NewEmailAddress = "xx@xx.xx",
                Notifications = false
            };
            _svc.Setup(
                m => m.UpdateUser(
                    UserVm.FormerEmailAddress,
                    UserVm.NewEmailAddress, 
                    UserVm.Notifications)).Returns(new User());

            // When
            _Controller.ChangeEmail(UserVm);
            
            // Then 
            _svc.Verify(r => r.UpdateUser(
                    UserVm.FormerEmailAddress,
                    UserVm.NewEmailAddress, 
                    UserVm.Notifications), Times.Once());
        }
         
        [Fact]
        public void UserController_ChangeEmail_ValidViewModel_Redirects()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new EmailViewModel(){
                FormerEmailAddress = "xx@xx.xx",
                NewEmailAddress = "xx@xx.xx",
                Notifications = false
            };
            _svc.Setup(
                m => m.UpdateUser(
                    UserVm.FormerEmailAddress,
                    UserVm.NewEmailAddress, 
                    UserVm.Notifications)).Returns(new User());

            // When
            var Result = (RedirectToActionResult) _Controller.ChangeEmail(UserVm) ;
            var RouteValue = (Object[]) Result.RouteValues.Values;
            // Then 
            Assert.Equal("User", Result.ControllerName);
            Assert.Equal("UserSettings", Result.ActionName);
            Assert.Equal("xx@xx.xx", RouteValue[0].ToString()); 
        }

        [Fact]
        public void UserController_ChangePassword_InvalidEmail_ReturnsNotFound()
        {
            // Given
            string Email = "XX@XX.XX";

            User NullUser = null;
            _svc.Setup(
                m => m.GetAccountByEmail(Email)).Returns(NullUser);
            
            // When
            IActionResult Result =  _Controller.ChangePassword(Email);
            
            // Then 
            Assert.IsType<NotFoundResult>(Result);
        }

        [Fact]
        public void UserController_ChangePassword_ValidEmail_ReturnsViewModel()
        {
            // Given
            string Email = "XX@XX.XX";

            User User = new User(){
                EmailAddress = "xx@xx.xx",
            };
            _svc.Setup(
                m => m.GetAccountByEmail(Email)).Returns(User);
            
            // When
            var Result =  _Controller.ChangePassword(Email) as ViewResult;
            var Model = (PasswordViewModel) Result.ViewData.Model;
            
            // Then 
            Assert.Equal(User.EmailAddress, Model.EmailAddress);
        }

        [Fact]
        public void UserController_ChangePassword_InValidViewModel_ReturnsViewModel()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new PasswordViewModel()
            { 
                EmailAddress = "xx@xx.xx",
                NewPassword = "XXX" 
                };
            _svc.Setup(
                m => m.UpdatePassword(
                    UserVm.EmailAddress,
                    UserVm.NewPassword)).Returns(true);
            _Controller.ModelState.AddModelError("Oh Oh", "Oh, oh");

            // When
            var Result =  _Controller.ChangePassword(UserVm) as ViewResult;
            var Model = (PasswordViewModel) Result.ViewData.Model;
            
            // Then 
            Assert.Equal(UserVm, Model);
        }

        [Fact]
        public void UserController_ChangePassword_InValidViewModel_DoesNotCallService()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new PasswordViewModel()
            { 
                EmailAddress = "xx@xx.xx",
                NewPassword = "XXX" 
                };
            _svc.Setup(
                m => m.UpdatePassword(
                    UserVm.EmailAddress,
                    UserVm.NewPassword)).Returns(true);
            _Controller.ModelState.AddModelError("Oh Oh", "Oh, oh");

            // When
            var Result =  _Controller.ChangePassword(UserVm) as ViewResult;
            var Model = (PasswordViewModel) Result.ViewData.Model;
            
            // Then 
            _svc.Verify(r => r.UpdatePassword(
                    UserVm.EmailAddress,
                    UserVm.NewPassword), Times.Never());
        }

        [Fact]
        public void UserController_ChangePassword_ValidViewModel_CallsService()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new PasswordViewModel()
            { 
                EmailAddress = "xx@xx.xx",
                NewPassword = "XXX" 
                };
            _svc.Setup(
                m => m.UpdatePassword(
                    UserVm.EmailAddress,
                    UserVm.NewPassword)).Returns(true);

            // When
            _Controller.ChangePassword(UserVm);
            
            
            // Then 
            _svc.Verify(r => r.UpdatePassword(
                    UserVm.EmailAddress,
                    UserVm.NewPassword), Times.Once());
        }
         
        [Fact]
        public void UserController_ChangePassword_ValidViewModel_Redirects()
        {
            // Given
            _Controller.TempData =
            new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            var UserVm = new PasswordViewModel()
            { 
                EmailAddress = "xx@xx.xx",
                NewPassword = "XXX" 
                };
            _svc.Setup(
                m => m.UpdatePassword(
                    UserVm.EmailAddress,
                    UserVm.NewPassword)).Returns(true);

            // When
            var Result = (RedirectToActionResult) _Controller.ChangePassword(UserVm) ;
            var RouteValue = (Object[]) Result.RouteValues.Values;
            // Then 
            Assert.Equal("User", Result.ControllerName);
            Assert.Equal("UserSettings", Result.ActionName);
            Assert.Equal("xx@xx.xx", RouteValue[0].ToString()); 
        }

    }// Class
}