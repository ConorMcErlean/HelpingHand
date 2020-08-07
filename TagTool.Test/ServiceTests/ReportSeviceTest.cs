using System;
using Xunit;
using TagTool.Data.Services;
using TagTool.Data.Repositories;
using TagTool.Data.Models;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;

namespace TagTool.Test
{
    public class ReportServiceTest
    {
        /* Service Being Tested */
        private readonly IReportService _svc;
        /* Dependencies */
        private readonly Mock<IRestService> restService;
        private readonly Mock<ICityService> cityService;
       
        /* Database for checks on database */
        private readonly DataContext _db;

        public ReportServiceTest()
        {
            // general arrangement
            _db = new TestContext().GetContext("ReportTest");

            restService = new Mock<IRestService>();
            cityService = new Mock<ICityService>();
          
            var Response = new What3WordsResponse
            {
                words = "Three.Word.Address",
                nearestPlace = "SomeOtherPlace"
            };
            var Cities = new List<City>
            {
            new City{ CityID = 1, Name ="City"},
            new City{ CityID = 2, Name ="Alternative City"}
            };
            restService.Setup(m => m.GetWhat3WordsAsync(It.IsAny<Double>(), It.IsAny<Double>())).Returns(Task.FromResult(Response));
            cityService.Setup(m => m.GetCities()).Returns(Cities);
            cityService.Setup(m => m.GetOutsideCity()).Returns(new City { CityID = 3, Name = "NonCity" }); 
            _svc = new ReportService(restService.Object, cityService.Object, _db);

            // ensure database is empty before each test
            _db.Initialise();
        }// Constructor

        /* New Report Tests */
        [Fact]
        public async void NewReport_WhereFieldsCorrect_ReturnShouldBeTypeReport()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";

            // When
            var Return = await _svc.NewReport(lat, lng, info);
           
            // Then
            Assert.IsType<Report>(Return);
        }

        [Fact]
        public async void NewReport_WhereFieldsCorrect_CreatedShouldHavePassedParameters()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";

            // When
            var Report = await _svc.NewReport(lat, lng, info);

            // Then
            Assert.Equal(lng, Report.Longitude);
            Assert.Equal(lat, Report.Latitude);
            Assert.Equal(info, Report.AdditionalInfo);
        }

        [Fact]
        public async void NewReport_WhereFieldsCorrect_CreatedShouldHaveCreatedTime()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";

            // When
            var Report = await _svc.NewReport(lat, lng, info);

            // Then
            Assert.Equal(DateTime.Now, Report.CreatedAt, TimeSpan.FromSeconds(10.00));
        }

        [Fact]
        public async void NewReport_WhereFieldsCorrect_CreatedShouldHaveActivePropertySet()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";

            // When
            var Report = await _svc.NewReport(lat, lng, info);

            // Then
            Assert.True(Report.Active);
        }

        [Fact]
        public async void NewReport_WhereFieldsCorrect_CreatedShouldAddToDatabase()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";

            // When
            var Report = await _svc.NewReport(lat, lng, info);
            var Retrieved = _db.Reports.FirstOrDefault(r => r.ReportID == Report.ReportID);

            // Then
            Assert.NotNull(Retrieved);
            Assert.Equal(Report.Longitude, Retrieved.Longitude);
            Assert.Equal(Report.Latitude, Retrieved.Latitude);
            Assert.Equal(Report.AdditionalInfo, Retrieved.AdditionalInfo);
        }

        [Fact]
        public async void NewReport_WhereInfoFieldBlank_ShouldCreateAsNormal()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = null;

            // When
            var Report = await _svc.NewReport(lat, lng, info);

            // Then
            Assert.NotNull(Report);
        }



        /* GetById Tests */

        [Fact]
        public async void GetById_ValidID_ReturnShouldBeTypeReport()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Retrieved = _svc.GetById(Report.ReportID);

            // Then
            Assert.IsType<Report>(Retrieved);
        }

        [Fact]
        public async void GetById_ValidID_ShouldReturnRequestedReport()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Retrieved = _svc.GetById(Report.ReportID);

            // Then
            Assert.Equal(Report.Latitude, Retrieved.Latitude);
            Assert.Equal(Report.Longitude, Retrieved.Longitude);
            Assert.Equal(Report.AdditionalInfo, Retrieved.AdditionalInfo);
            Assert.Equal(Report.CreatedAt, Retrieved.CreatedAt);
        }

        [Fact]
        public async void GetById_InValidID_ReturnShouldBeNull()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Retrieved = _svc.GetById(Report.ReportID + 1);

            // Then
            Assert.Null(Retrieved);
        }

        /* Mark Report Complete Tests */

        [Fact]
        public async void MarkReportComplete_ValidReport_ReturnShouldBeTypeReport()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Completed = _svc.MarkReportComplete(Report);

            // Then
            Assert.IsType<Report>(Completed);
        }

        [Fact]
        public async void MarkReportComplete_ValidReport_ReportShouldBeReturned()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Completed = _svc.MarkReportComplete(Report);

            // Then
            Assert.Equal(Report.Latitude, Completed.Latitude);
            Assert.Equal(Report.Longitude, Completed.Longitude);
            Assert.Equal(Report.AdditionalInfo, Completed.AdditionalInfo);
            Assert.Equal(Report.CreatedAt, Completed.CreatedAt);
        }

        [Fact]
        public async void MarkReportComplete_ValidReport_ReportValidityShouldBeSetFalse()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Completed = _svc.MarkReportComplete(Report);

            // Then
            Assert.False(Completed.Active);
        }

        [Fact]
        public void MarkReportComplete_InValidReport_ReturnShouldBeNull()
        {
            // Given

            Report Report = null;

            // When
            var Completed = _svc.MarkReportComplete(Report);

            // Then
            Assert.Null(Completed);
        }

        /* Delete Tests */

        [Fact]
        public async void DeleteReport_ValidReport_ShouldReturnTrue()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Deleted = _svc.Delete(Report);

            // Then
            Assert.True(Deleted);
        }

        [Fact]
        public async void DeleteReport_ValidReport_ShouldRemoveReportFromDatabase()
        {
            // Given
            double lng = 45.00;
            double lat = 45.00;
            string info = "TEXT";
            var Report = await _svc.NewReport(lat, lng, info);

            // When
            var Deleted = _svc.Delete(Report);
            var Retrieved = _db.Reports.FirstOrDefault(r => r.ReportID == Report.ReportID);

            // Then
            Assert.Null(Retrieved);
        }

        [Fact]
        public void DeleteReport_InValidReport_ShouldReturnFalse()
        {
            // Given

            Report Report = null;

            // When
            var Deleted = _svc.Delete(Report);

            // Then
            Assert.False(Deleted);
        }

        [Fact]
        public void GetAllReports_NoReports_ShouldReturnEmptyList()
        {
            // Given
            string orderby = null;

            // When
            var Reports = _svc.GetAllReports(orderby);

            // Then
            Assert.Empty(Reports);
        }


        [Fact]
        public async void GetAllReports_Reports_ShouldReturnListOfCorrectLength()
        {
            // Given
            var Report1 = await _svc.NewReport(10.00, -10.00, "Info");
            var Report2 = await _svc.NewReport(-75, 75, "Report2");
            var Report3 = await _svc.NewReport(45.00007, 45.00006, "ABCDEF");
            string orderby = null;

            // When
            var Reports = _svc.GetAllReports(orderby);

            // Then
            Assert.True(Reports.Count() == 3);
        }

        [Fact]
        public async void GetAllReports_Reports_ShouldReturnListCorrectOrder()
        {
            // Given
            var Report1 = await _svc.NewReport(10.00, -10.00, "Info");
            var Report2 = await _svc.NewReport(-75, 75, "Report2");
            var Report3 = await _svc.NewReport(45.00007, 45.00006, "ABCDEF");
            string orderby = null;

            // When
            var NullReports = _svc.GetAllReports(orderby);
            var NullExpected = _db.Reports.OrderByDescending(r => r.CreatedAt).ToList();

            orderby = "Created";
            var CreatedReports = _svc.GetAllReports(orderby);
            var CreatedExpected = _db.Reports.OrderBy(r => r.CreatedAt).ToList();

            orderby = "Active";
            var ActiveReports = _svc.GetAllReports(orderby);
            var ActiveExpected = _db.Reports.OrderBy(r => r.Active).ToList();

            // Then
            for (int i = 0; i < CreatedReports.Count(); i++)
            {
                Assert.Equal(NullExpected[i].ReportID, NullReports[i].ReportID);
                Assert.Equal(CreatedExpected[i].ReportID, CreatedReports[i].ReportID);
                Assert.Equal(ActiveExpected[i].ReportID, ActiveReports[i].ReportID);
            }
        }

        [Fact]
        public void GetReportsJSON_NoReports_ShouldReturnEmptyList()
        {
            // Given
            
            // When
            var json = _svc.GetReportsJSON();

            // Then
            Assert.Equal("[]", json);
        }

        [Fact]
        public async void GetReportsJSON_Reports_ShouldReturnString()
        {
            // Given
            var Report1 = await _svc.NewReport(10.00, -10.00, "Info");
            var Report2 = await _svc.NewReport(-75, 75, "Report2");
            var Report3 = await _svc.NewReport(45.00007, 45.00006, "ABCDEF");
            String CorrectJSONString = "[{\"name\":\"Three.Word.Address\",\"lat\":10,\"lon\":-10,\"country\":\"GB\"},{\"name\":\"Three.Word.Address\",\"lat\":-75,\"lon\":75,\"country\":\"GB\"},{\"name\":\"Three.Word.Address\",\"lat\":45.00007,\"lon\":45.00006,\"country\":\"GB\"}]";

            // When
            var json = _svc.GetReportsJSON();

            // Then
            Assert.Equal(CorrectJSONString, json);
        }
    }// Class
}// Namespace
