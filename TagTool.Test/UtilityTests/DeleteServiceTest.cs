using TagTool.Data.Services;
using TagTool.Data.Repositories;
using TagTool.Data.Models;
using System.Linq;
using Xunit;
using System;

namespace TagTool.Test
{
    public class DeleteServiceTest
    {
        /* Service Being Tested */
        private readonly IDeleteService _svc;
 
        /* Dependencies */
        private readonly ICityService _city;
      
        

        /* Database for checks on database */
        private readonly DataContext _db;

        public DeleteServiceTest()
        {
            // general arrangement
            _db = new TestContext().GetContext("DeleteTest");
            _svc = new DeleteService(_db);
            _city = new CityService(_db);
        }

        /* Test Cases */

        [Fact]
        public async void DeleteService_ShouldRemoveAll_IfReportsOlderThan3Hours()
        {
            // Given
            _db.Initialise();
            
           _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-3),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-4),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-6),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.SaveChanges();

            // When
            await _svc.CleanRecordsASync();

            // Then
            Assert.Empty(_db.Reports.ToList());
        }

        [Fact]
        public async void DeleteService_ShouldNotRemoveAny_IfReportsYoungerThan3Hours()
        {
            // Given
            _db.Initialise();
            
           _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-2),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-1),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = DateTime.Now,
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.SaveChanges();

            // When
            await _svc.CleanRecordsASync();

            // Then
            Assert.Equal(3, _db.Reports.ToList().Count);
        }

        [Fact]
        public async void DeleteService_ShouldOnlyRemove_IfReportsOld()
        {
            // Given
            _db.Initialise();
            
           _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-3),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-5),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = (DateTime.Now).AddHours(-2),
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );

            _db.Add( 
               new Report
                {
                    Latitude = 54.597216,
                    Longitude = -5.930420,
                    CreatedAt = DateTime.Now,
                    AdditionalInfo = "Report from Belfast City Hall",
                    Active = true,
                    ThreeWordAddress = "twigs.purple.pulled",
                    City = _city.GetOutsideCity()
                }
            );
            _db.SaveChanges();

            // When
            await _svc.CleanRecordsASync();

            // Then
            Assert.Equal(2, _db.Reports.ToList().Count);
        }
    }
}