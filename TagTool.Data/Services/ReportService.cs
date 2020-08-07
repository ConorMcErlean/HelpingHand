using TagTool.Data.Repositories;
using TagTool.Data.Models;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.IO;

namespace TagTool.Data.Services
{
    /* Conncrete implementation of IReportService */
    public class ReportService : IReportService
    {
        /* Dependencies */
        private readonly DataContext _db;
        private readonly IRestService _rs;
        private readonly ICityService _Cities;
    
       
        /* Constuctor with Constructor DI-injection */
        public ReportService(IRestService rs, ICityService Cities, DataContext db)
        {
            _db = db;
            _rs = rs;
            _Cities = Cities;
        }

        /* Method to call Database initialisation method. */
        public void Initialise(){
            _db.Initialise();
        }

        /* Method to generate a new report */
        public async Task<Report> NewReport(double Latitude, double Longitude, string Info)
        {
            var Report = new Report
            {
                Latitude = Latitude,
                Longitude = Longitude,
                AdditionalInfo = Info,
                CreatedAt = DateTime.Now,
                Active = true
            };

            // Get Three Word Address
            var response = await _rs.GetWhat3WordsAsync(Latitude, Longitude);

            // Ingest response into report
            Report.ThreeWordAddress = response.words;

            // Add City
            var cities = _Cities.GetCities();
            bool CitySet = false;
            foreach (var City in cities)
            {
                if (City.Name == response.nearestPlace)
                {
                    Report.City = City;
                    CitySet = true;
                }
            }
            if (!CitySet)
            {
                Report.City = _Cities.GetOutsideCity();
            }

            // Add to database
            await _db.Reports.AddAsync(Report);
            await _db.SaveChangesAsync();

             // Notify
            await Task.Run(() => Notify(Report));

            return Report;
        }

        /* Method to get a list of all Reports with order by parameter */
        public IList<Report> GetAllReports(string OrderBy = null)
        {
            switch (OrderBy)
            {
                case "Created":
                    return _db.Reports
                    .Include(r => r.City)
                    .OrderBy(r => r.CreatedAt).ToList();
                case "Active":
                    return _db.Reports
                    .Include(r => r.City)
                    .OrderBy(r => r.Active).ToList();
                default:
                    return _db.Reports
                    .Include(r => r.City)
                    .OrderByDescending(r => r.CreatedAt).ToList();
            }
        }

        /* Method to get JSON list of reports */
        public string GetReportsJSON(){
            
            // Get List and format to list of properties
            var Reports = _db.Reports.ToList();
            List<object> Template = new List<object>();
            foreach(Report r in Reports){
                Template.Add( new {
                    name = r.ThreeWordAddress,
                    lat = r.Latitude,
                    lon = r.Longitude,
                    country = "GB"
                });
            }

            var options = new JsonSerializerOptions
                {
                };
            var JSON = JsonSerializer.Serialize(Template, options);
            return JSON;
        }

        /* Method to get a report by its ID */
        public Report GetById(int Id)
        {
            return _db.Reports.FirstOrDefault(r => r.ReportID == Id);
        }

        /* Method to mark a report as dealt-with */
        public Report MarkReportComplete(Report Report)
        {
            if (Report == null){return null;}
            Report.Active = false;
            _db.Reports.Update(Report);
            _db.SaveChanges();
            return Report;
        }

        /* Method to delete a report */
        public bool Delete(Report report)
        {
            if (report == null) { return false;}
            _db.Reports.Remove(report);
            _db.SaveChanges();
            return true;
        }

        private void Notify(Report Report)
        {
            var MailList = _db.Users.Where(u => (u.CityID == Report.CityID) &&
                u.EmailNotifications).ToList();
            // Message
            if (MailList.Count == 0){return;}
            string City = _Cities.GetCity(Report.CityID).Name;
            var Response = EmailService.SendMessage(MailList, Report, City);
        }
    }
}