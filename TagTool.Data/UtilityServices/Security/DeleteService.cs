using System;
using TagTool.Data.Models;
using TagTool.Data.Repositories;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TagTool.Data.Services
{
    public class DeleteService : IDeleteService
    {
        // Required by the Delete service
        private readonly DataContext _db;
        
    
        public DeleteService(DataContext db)
        {
            _db = db;
        }

        // The API call using System.net
        public async Task CleanRecordsASync()
        {
            /* Change the value below to adjust how long data is kept */
            int Hours = 3;
            // Get older Records
            var outdated = getReports(Hours);
            
            // For each record older than timespan remove
            foreach( Report r in outdated)
            {
                _db.Remove(r);
            }
            // Save changes
            await _db.SaveChangesAsync();
        }

        private IList<Report> getReports(int MaxAgeInHours)
        {
            TimeSpan maxLife = new TimeSpan( MaxAgeInHours, 0, 0);
            /* Set Timespan for record life */
            DateTime cutoff = DateTime.Now.Subtract(maxLife);

            /* Check Database for all records older than timespan */
            var outdated = _db.Reports.Where
                (r => 0 >= DateTime.Compare(r.CreatedAt, cutoff)).ToList();
            return outdated;
        }
    }// RestClientClass
}