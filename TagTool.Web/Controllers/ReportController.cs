using System;
using TagTool.Data.Models;
using TagTool.Data.Services;
using TagTool.Data.Repositories;
using TagTool.Data.Secrets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using TagTool.Web.Models;

namespace TagTool.Web.Controllers
{
    /*
    Using:
    https://auth0.com/blog/dependency-injection-in-dotnet-core/
    */

    [Authorize]
    public class ReportController : BaseController
    {
        private readonly IReportService _svc;
        private readonly DataContext _DbContext;

        public ReportController(DataContext dbContext, IReportService svc)
        {
            _DbContext = dbContext;
            _svc = svc;
        }

        // Post /Report/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            var rvm = new ReportViewModel()
            {
                GoogleAPIKey = GetKey.GoogleAPIKey()
            };
            return View(rvm);
        }

        // Post /Report/Create
        [HttpPost] [AllowAnonymous] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportViewModel rvm)
        {
            if (ModelState.IsValid)
            {
                var Report = await _svc.NewReport(rvm.Latitude, rvm.Longitude, 
                rvm.AdditionalInfo);
                Alert("Report Submitted.", AlertType.success, "Thank you so much!");
                return RedirectToAction("Index", "Home");
            }
            Alert("Issue submitting report.", AlertType.warning, "Uh oh!");
            return View(rvm);
        }

        // Get /Reports
        public IActionResult ViewReports(string OrderBy)
        {
            var Reports = _svc.GetAllReports(OrderBy);
            return View(Reports);
        }

        // Get /ViewReport/{ID}
        public IActionResult ViewReport(int Id)
        {
            var Report = _svc.GetById(Id);
            if (Report == null){ return NotFound(); }
            TempData["MapsAPIKey"] = GetKey.GoogleAPIKey();
            return View(Report);
        }   

        // Post /Report/{ID}/Complete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkReportComplete(int ReportID)
        {
            var Report = _svc.GetById(ReportID);
            if (Report == null)
            {
                Alert("Report Not Found", AlertType.warning);
                return RedirectToAction("ViewReports", "Report");
            }
            _svc.MarkReportComplete(Report);
            Alert("Report marked complete!", AlertType.success);
            return RedirectToAction("ViewReports", "Report");;
        }

        // Get /Report/{ID}/ConfirmDelete
        public IActionResult Delete(int Id)
        {
            var Report = _svc.GetById(Id);
            if (Report == null)
            {
                Alert("Report Not Found", AlertType.warning);
                return RedirectToAction("ViewReports", "Report");
            }
            Alert("Report will Be permanently deleted, are you sure?", 
                AlertType.danger);
            return View(Report);
        }

         // Post /Repor/{ID}/ConfirmDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Post /Vehicle/Delete
        public IActionResult ConfirmDelete(int ReportID)
        {
            var Remove = _svc.GetById(ReportID);
            _svc.Delete(Remove);  
            Alert("Report Deleted.", AlertType.success);
            return RedirectToAction("ViewReports", "Report");
        }

        public IActionResult ReportsMap()
        {
            var jsonReports = _svc.GetReportsJSON();
            return View(model:jsonReports);
        }
    }
}