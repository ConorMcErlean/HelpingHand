
using TagTool.Data.Models;
using TagTool.Data.Services;
using TagTool.Data.Repositories;
using TagTool.Web.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace TagTool.Web.Controllers
{

    public class UserController : BaseController
    {
        private readonly IAccountService _svc;
        private readonly DataContext _DbContext;
        private readonly ICityService _Csvc;
        

        public UserController(DataContext DbContext, IAccountService svc, ICityService CSvc)
        {
            _DbContext = DbContext;
            _svc = svc;
            _Csvc = CSvc;
        }

        // Get /User/Register
        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            var Cities = _Csvc.GetCities();
            var rvm = new RegisterViewModel
                {
                    Cities = new SelectList(Cities, "CityID", "Name"),
                };
            return View(rvm);
        }

        // Post /User/Register
        [ValidateAntiForgeryToken] [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                
                var user = _svc.Register(
                    rvm.EmailAddress,
                    rvm.Name,
                    rvm.GetRole(),
                    rvm.Password,
                    _Csvc.GetCity(rvm.CityID)
                );
                if (user != null)
                {
                    Alert("User \"" + user.EmailAddress + "\" Created.",
                    AlertType.success);
                    return RedirectToAction("AdminHome", "Home");
                }
            }
            Alert("Oh. Something wasn't right there." ,
                 AlertType.warning);
            // Repopulate Selectlist
            var Cities = _Csvc.GetCities();
            rvm.Cities = new SelectList(Cities, "CityID", "Name");
            return View(rvm);
        }

        // Get /User/Login 
        public IActionResult Login()
        {
            return View();
        }

        // Post /User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("EmailAddress,Password")]LoginUserViewModel m)
        { 
            // call service to Autheticate User
            var user = _svc.Authenticate(m.EmailAddress, m.Password);  
            
            // verify if user found and if not then add a model state e
            if (user == null)
            {
                ModelState.AddModelError("EmailAddress", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }  
               
            // sign user in using cookie authentication to store principal
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                BuildClaimsPrincipal(user)
            );
            if (user.Role == Role.Admin){return RedirectToAction("AdminHome","Home");}
            return RedirectToAction("ViewReports","Report");
            }

        // Get /User/Close{ID}
        [Authorize(Roles = "Admin")]
        public IActionResult ViewAccounts(string OrderBy)
        {
            var List = _svc.ViewAllAccounts(OrderBy);
            return View(List);
        }
        // Get /User/Close{ID}
        [Authorize(Roles = "Admin")]
        public IActionResult CloseAccount(string EmailAddress)
        {
            var User = _svc.GetAccountByEmail(EmailAddress);
            if (User == null){ return NotFound();}
            var uvm = new CloseUserViewModel
            {
                EmailAddress = User.EmailAddress,
                Name = User.Name,
                UserID = User.UserID,
                Role = User.Role
            };
            return View(uvm);
        }

        // Post /User/Close{ID}
        [ValidateAntiForgeryToken] [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CloseAccount(CloseUserViewModel uvm)
        {
            var User = _svc.GetAccountByEmail(uvm.EmailAddress);
            _svc.CloseAccount(User);
            Alert("User Removed.", AlertType.success);
            return RedirectToAction("Index", "Home");
        }


        // Get /User/City/Create
        [Authorize(Roles = "Admin")]
        public IActionResult CreateCity()
        {
            return View();
        }

        // Post /User/City/Create
        [ValidateAntiForgeryToken] [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateCity(CityViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                var created = _Csvc.CreateCity(cvm.Name);
                if (created != null)
                {
                    Alert("New City added!", AlertType.success);
                    return RedirectToAction("Index", "Home");
                }
            }
            Alert("We ran into an issue.", AlertType.warning);
            return View(cvm);
        }

        // Post /User/logout
        [ValidateAntiForgeryToken] [Authorize(Roles = "Admin, User")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData.Clear();
            return RedirectToAction("Index", "Home");
        }

        [ValidateAntiForgeryToken]
        public IActionResult ErrorUserNotAuthorized(){return null;}

        [ValidateAntiForgeryToken]
        public IActionResult ErrorUserNotAuthenticated(){return null;}
         
        // Build a claims principal from authenticated user
        private ClaimsPrincipal BuildClaimsPrincipal(User user)
        { 
            // define user claims
            var claims = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role.ToString())                              
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            // build principal using claims
            return  new ClaimsPrincipal(claims);
        }


        // Access User Settings
        [Authorize]
        public IActionResult UserSettings(string Email)
        {
            // Pass Email to view
            return View(model:Email);
        }

        [Authorize]
        public IActionResult ChangeEmail(string Email)
        {
            var User = _svc.GetAccountByEmail(Email);
            if (User == null){ return NotFound();}
            var vm = new EmailViewModel
            {
                FormerEmailAddress = User.EmailAddress,
                Notifications = User.EmailNotifications,
                NewEmailAddress = User.EmailAddress
            };
            return View(vm);
        }

        // Post /User/UserSettings/ChangeEmail
        [ValidateAntiForgeryToken] [Authorize] [HttpPost]
        public IActionResult ChangeEmail(EmailViewModel vm)
        {
            if (ModelState.IsValid)
            {  
                _svc.UpdateUser(
                    vm.FormerEmailAddress, 
                    vm.NewEmailAddress, 
                    vm.Notifications
                    );
                Alert("Account Updated!" ,
                 AlertType.success);
                string Email = vm.NewEmailAddress.ToString();   
                return RedirectToAction("UserSettings", "User" , new{Email});
            }
            Alert("Oh. Something wasn't right there." ,
                 AlertType.warning);
            return View(vm);
        }


        [Authorize]
        public IActionResult ChangePassword(string Email)
        {
            var User = _svc.GetAccountByEmail(Email);
            if (User == null) { return NotFound();}
            var vm = new PasswordViewModel
            {
                EmailAddress = User.EmailAddress
            };
            ;
            return View(vm);
        }

        // Post /User/UserSettings/ChangeEmail
        [ValidateAntiForgeryToken] [Authorize] [HttpPost]
        public IActionResult ChangePassword(PasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {  
                var Result = _svc.UpdatePassword(
                    vm.EmailAddress,
                    vm.NewPassword);
                Alert("Password Updated" ,
                 AlertType.success);
                 string Email = vm.EmailAddress.ToString();
                 return RedirectToAction("UserSettings", "User" , new{Email});
            
            }
            Alert("Oh. Something wasn't right there." ,
                 AlertType.warning);
            return View(vm);
        }

        //  
        public IActionResult SettingsVerify()
        {
            return View();
        }

        // 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SettingsVerify([Bind("EmailAddress,Password")]LoginUserViewModel m)
        { 
            // call service to Autheticate User
            var user = _svc.Authenticate(m.EmailAddress, m.Password);  
            
            // verify if user found and if not then add a model state e
            if (user == null)
            {
                ModelState.AddModelError("EmailAddress", "Invalid Login Credentials");
                ModelState.AddModelError("Password", "Invalid Login Credentials");
                return View(m);
            }  
            string Email = m.EmailAddress;
            return RedirectToAction("UserSettings", "User" , new {Email} );
        }
    }
}