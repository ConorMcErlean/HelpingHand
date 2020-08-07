using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using TagTool.Web.Models;

namespace TagTool.Web.Controllers
{
    public class HomeController : BaseController
    {
        /* Dependencies */
        private readonly ILogger<HomeController> _logger;

        /* Constructor */
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /* 
        Home routes to Index View if regular user, but routes to relevant screen
        for autherised users
        */
        public IActionResult Index()
        {
            return View();
        }

        /* Routes to admin home */
        [Authorize(Roles = "Admin")]
        public IActionResult AdminHome()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
