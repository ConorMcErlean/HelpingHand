using TagTool.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TagTool.Data.Services
{
    /* 
    The abstraction of ReportSevice, allows the service to be injected through 
    dependency injection.
    */
    public interface IReportService
    {
        /* Method to initialise the Database */
        void Initialise();

        /* Method to generate a new report */
        Task<Report> NewReport(double Latitude, double Longitude, string Info);

        /* Method to get all reports with orderby String*/
        IList<Report> GetAllReports(string OrderBy);

        /* Method to get report as JSON string */
        string GetReportsJSON();

        /* Method to get a report by its ID */
        Report GetById(int Id);

        /* Method to mark a report as dealt-with */
        Report MarkReportComplete(Report Report);

        /* Method to delete a report */
        bool Delete(Report report);
    }
}