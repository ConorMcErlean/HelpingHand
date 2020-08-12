using TagTool.Web.Controllers;
using TagTool.Web.Models;
using TagTool.Data.Repositories;
using TagTool.Data.Services;
using TagTool.Data.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System;

namespace TagTool.Test
{
    public class ReportControllerTest
    {

        /*
        The following file contains all unit tests on the Report Controller
        in TagTool.Web.Controllers.

        */

        private readonly ReportController _Controller;
        private readonly Mock<IReportService> _svc;
        private readonly DataContext _db;
        private readonly HttpContext httpContext;

        public ReportControllerTest(){
            _svc = new Mock<IReportService>();
            _db = new TestContext().GetContext("ReportControllerTest");
            _Controller = new ReportController(_db,_svc.Object);
            // Create Temp Data Disctionary So Alerts dont throw errors.
            httpContext = new DefaultHttpContext();
        }

        [Fact]
        public void ReportController_Create_ReturnsView()
        {
            // When
            var Page = _Controller.Create();

            // Then
            Assert.IsType<ViewResult>(Page);
        }

        [Fact]
        public async void ReportController_Create_ValidViewModel_CallsNewReport()
        {
            // Given
                // Create View Model
            var rvm = new ReportViewModel(){
                Latitude = 45.00,
                Longitude = 45.00,
                AdditionalInfo = "Testing"
            };

                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.NewReport( 45.00,  45.00, "Testing"))
                .Returns(Task.FromResult(new Report()));
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            var Page = await _Controller.Create(rvm);

            // Then
            _svc.Verify(r => r.NewReport(45.00, 45.00, "Testing"), Times.Once());
        }

        [Fact]
        public async void ReportController_Create_InValidViewModel_ReturnsView()
        {
            // Given
                // Create Invalid View Model
            var rvm = new ReportViewModel()
                {   
                    Latitude = 45.00, 
                    Longitude = 45.00,
                    AdditionalInfo = "Test"
                };
           
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.NewReport( 45.00,  45.00, "Testing"))
                .Returns(Task.FromResult(new Report()));
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            _Controller.ModelState.AddModelError("Latitude", "Latitude is Required");
            var Page = await _Controller.Create(rvm) as ViewResult;
           
            // Then
                // Ensue New Report was never called
            _svc.Verify(r => r.NewReport(45.00, 45.00, "Testing"), Times.Never());

                // Ensure The View Model is returned
            var PageModel = (ReportViewModel) Page.ViewData.Model;
            Assert.Equal(45.00, PageModel.Latitude);
            Assert.Equal(45.00, PageModel.Longitude);
            Assert.Equal("Test", PageModel.AdditionalInfo);
        }

        [Fact]
        public void ReportController_ViewReports_ReturnsViewOrderedCorrectly()
        {
            // Given
                // Fake Reports, Only 2 Required as we only need to check the
                // Sevice is being called
            var r1 = new Report()
            {
                CreatedAt = DateTime.Now,
                Active = true 
            };
            var r2 = new Report()
            {
                CreatedAt = (DateTime.Now).AddHours(-1),
                Active = false
            };

                // Return Lists
            var List1 = new Report[]{r1, r2};
            var List2 = new Report[]{r2, r1};

                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetAllReports("TestCase1"))
                .Returns(List1);
            _svc.Setup(m => m.GetAllReports("TestCase2"))
                .Returns(List2);
            var _Controller = new ReportController(_db,_svc.Object);

            // When
            var Result1 = _Controller.ViewReports("TestCase1") as ViewResult;
           
            // Then
                // Ensue View Report was called once for each
            _svc.Verify(r => r.GetAllReports("TestCase1"), Times.Once());
            _svc.Verify(r => r.GetAllReports("TestCase2"), Times.Never());

                // Ensure The Reports List is returned
            var PageModel1 = (Report[]) Result1.ViewData.Model;
            Assert.Equal(List1, PageModel1);
            
        }

        [Fact]
        public void ReportController_ViewReports_DifferentOrder_ReturnsViewOrderedCorrectly()
        {
            // Given
                // Fake Reports, Only 2 Required as we only need to check the
                // Sevice is being called
            var r1 = new Report()
            {
                CreatedAt = DateTime.Now,
                Active = true 
            };
            var r2 = new Report()
            {
                CreatedAt = (DateTime.Now).AddHours(-1),
                Active = false
            };

                // Return Lists
            var List1 = new Report[]{r1, r2};
            var List2 = new Report[]{r2, r1};

                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetAllReports("TestCase1"))
                .Returns(List1);
            _svc.Setup(m => m.GetAllReports("TestCase2"))
                .Returns(List2);
            var _Controller = new ReportController(_db,_svc.Object);

            // When
            var Result2 = _Controller.ViewReports("TestCase2") as ViewResult;
           
            // Then
                // Ensue View Report was called once for each
            _svc.Verify(r => r.GetAllReports("TestCase1"), Times.Never());
            _svc.Verify(r => r.GetAllReports("TestCase2"), Times.Once());

                // Ensure The Reports List is returned
            var PageModel2 = (Report[]) Result2.ViewData.Model;
            Assert.Equal(List2, PageModel2);
        }

        [Fact]
        public void ReportController_ViewReport_WithID_ReturnsViewWithReport()
        {
            // Given  
            var r1 = new Report(){ CreatedAt = DateTime.Now, Active = true };
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(r1);
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            var Result = _Controller.ViewReport(1) as ViewResult;
           
            // Then
                // Ensue View Report was called once
            _svc.Verify(r => r.GetById(1), Times.Once());

                // Ensure The Report  is returned
            var PageModel = (Report) Result.ViewData.Model;
            Assert.Equal(r1, PageModel);
        }

        [Fact]
        public void ReportController_ViewReport_WithFalseID_ReturnsNotFound()
        {
            // Given  
            Report ReturnReport = null;
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(ReturnReport);
            var _Controller = new ReportController(_db,_svc.Object);

            // When
            IActionResult Result = _Controller.ViewReport(1);
            
            // Then
                // Ensue View Report was called once
            _svc.Verify(r => r.GetById(1), Times.Once());
                // Ensure Not Found  is returned
            Assert.IsType<NotFoundResult>(Result);
        }

        [Fact]
        public void MarkReportComplete_WithFalseID_DoesNotCallService()
        {
            // Given  
            Report ReturnReport = null;
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(ReturnReport);
            _svc.Setup(m => m.MarkReportComplete(ReturnReport))
                .Returns(ReturnReport);
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            IActionResult Result = _Controller.MarkReportComplete(1);
            
            // Then
                // Ensure Service is never called
            _svc.Verify(r => r.MarkReportComplete(ReturnReport), Times.Never());
        }

        [Fact]
        public void MarkReportComplete_WithID_CallsService()
        {
            // Given  
            Report ReturnReport = new Report(){Active = true};
            Report InActive = new Report(){Active = false};
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(ReturnReport);
            _svc.Setup(m => m.MarkReportComplete(ReturnReport))
                .Returns(InActive);
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            IActionResult Result = _Controller.MarkReportComplete(1);
            
            // Then
                // Ensure Service is called
            _svc.Verify(r => r.MarkReportComplete(It.IsAny<Report>()), Times.Once());
        }

        [Fact]
        public void Delete_WithID_ReturnsViewOfReport()
        {
            // Given  
            Report ReturnReport = new Report(){Active = true};
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(ReturnReport);
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            var Result = _Controller.Delete(1) as ViewResult;
            var Report = (Report) Result.ViewData.Model;

            // Then
                // Ensue Report Returned
            Assert.Equal(ReturnReport, Report);
        }

        [Fact]
        public void Delete_WithFalseID_RedirectsCorrectly()
        {
            // Given  
            Report ReturnReport = null;
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(ReturnReport);
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            var Result = (RedirectToActionResult) _Controller.Delete(1);
            
            // Then 
            Assert.Equal("ViewReports", Result.ActionName);
            Assert.Equal("Report", Result.ControllerName);
        }

        [Fact]
        public void ConfirmDelete_WithID_CallsService()
        {
            // Given  
            Report ReturnReport = new Report(){Active = true};
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(ReturnReport);
            _svc.Setup(m => m.Delete(ReturnReport))
                .Returns(true);   
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            var Result = _Controller.ConfirmDelete(1) as ViewResult;

            // Then
                // Ensure Service is called
            _svc.Verify(r => r.Delete(It.IsAny<Report>()), Times.Once());
        }

        [Fact]
        public void ConfirmDelete_WithID_Redirects()
        {
            // Given  
            Report ReturnReport = new Report(){Active = true};
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetById(1))
                .Returns(ReturnReport);
            _svc.Setup(m => m.Delete(ReturnReport))
                .Returns(true);   
            var _Controller = new ReportController(_db,_svc.Object);
            _Controller.TempData =
             new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // When
            var Result = (RedirectToActionResult) _Controller.ConfirmDelete(1);
            
            // Then 
            Assert.Equal("ViewReports", Result.ActionName);
            Assert.Equal("Report", Result.ControllerName);
        }

        [Fact]
        public void ReportsMap_CallsService()
        {
            // Given  
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetReportsJSON())
                .Returns("JSON String");   
            var _Controller = new ReportController(_db,_svc.Object);

            // When
            var Result = _Controller.ReportsMap() as ViewResult;
            
            // Then 
            _svc.Verify(r => r.GetReportsJSON(), Times.Once());
        }

        [Fact]
        public void ReportsMap_JsonInView()
        {
            // Given  
                // Adjust Mocked Components
            var _svc = new Mock<IReportService>();
            _svc.Setup(m => m.GetReportsJSON())
                .Returns("JSON String");   
            var _Controller = new ReportController(_db,_svc.Object);

            // When
            var Result = _Controller.ReportsMap() as ViewResult;
            var ViewJson = (string) Result.ViewData.Model;
            
            // Then 
            _svc.Verify(r => r.GetReportsJSON(), Times.Once());
        }
    }
}