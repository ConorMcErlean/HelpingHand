using System;
using Xunit;
using TagTool.Data.Services;
using TagTool.Data.Repositories;
using TagTool.Data.Models;
using System.Linq;

namespace TagTool.Test
{
    public class CityServiceTest
    {

        /* Service Being Tested */
        private readonly ICityService svc;
        /* Database for checks on database */
        private readonly DataContext _db;

        public CityServiceTest()
        {
            // general arrangement
            _db = new TestContext().GetContext("CityTest");
            svc = new CityService(_db);
            
            // ensure database is empty before each test
            _db.Initialise();
        }


        /* City Tests */

        [Fact]
        public void CreateCity_WhenAllFieldsFull_ReturnShouldBeTypeCity()
        {
            // Given
            String CityName = "TestCity";

            // When
            var Created = svc.CreateCity(CityName);

            // Then
            Assert.NotNull(Created);
            Assert.IsType<City>(Created);
        }// CreateCity_WhenAllFieldsFull_ReturnShouldBeTypeCity


        [Fact]
        public void CreateCity_WhenAllFieldsFull_CreatedShouldHaveCityID()
        {
            // Given
            String CityName = "TestCity";

            // When
            var Created = svc.CreateCity(CityName);

            // Then
            Assert.True(Created.CityID >= 0);
        }// CreateCity_WhenAllFieldsFull_CreatedCityShouldHaveCityID

        [Fact]
        public void CreateCity_WhenAllFieldsFull_CreatedShouldHavePassedParameters()
        {
            // Given
            String CityName = "TestCity";

            // When
            var Created = svc.CreateCity(CityName);

            // Then
            Assert.Equal(Created.Name, CityName);
        }// CreateCity_WhenAllFieldsFull_PassedNameShouldBeSet

        [Fact]
        public void CreateCity_WhenAllFieldsFull_CityShouldBeAddedToDatabase()
        {
            // Given
            String CityName = "TestCity";

            // When
            var Created = svc.CreateCity(CityName);
            var _dbCity = _db.Cities.FirstOrDefault(c => c.CityID == Created.CityID);
            
            // Then
            Assert.Equal(Created.CityID, _dbCity.CityID);
            Assert.Equal(Created.Name, _dbCity.Name);
        }//CreateCity_WhenAllFieldsFull_CityShouldBeAddedToDatabase

        [Fact]
        public void CreateCity_WhenCityExists_ShouldReturnNull()
        {
            // Given
            String CityName = "TestCity";
            String DuplicateName = "TestCity";

            // When
            var Exists = svc.CreateCity(CityName);
            var Created = svc.CreateCity(DuplicateName);

            // Then
            Assert.Null(Created);
        }// CreateCity_WhenCityExists_ShouldReturnNull

        [Fact]
        public void CreateCity_WhenCityExists_ShouldNotAddToDB()
        {
            // Given
            String CityName = "TestCity";
            String DuplicateName = "TestCity";

            // When
            var Exists = svc.CreateCity(CityName);
            var Created = svc.CreateCity(DuplicateName);

            // Then
            var Entries = _db.Cities.Select(c => c.Name == "TestCity");
            Assert.Equal( 1, Entries.Count());
        }// CreateCity_WhenCityExists_ShouldNotAddToDB

        [Fact]
        public void GetCity_WhenCityExists_ShouldReturnCity()
        {
            // Given
            String CityName = "TestCity";
            var Exists = svc.CreateCity(CityName);

            // When
            var Retrived = svc.GetCity(Exists.CityID);

            // Then
            Assert.NotNull(Retrived);
            Assert.IsType<City>(Retrived);
            Assert.Equal(Exists.Name, Retrived.Name);
            Assert.Equal(Exists.CityID, Retrived.CityID);
        }// 

        [Fact]
        public void GetCity_WhenCityDoesntExist_ShouldReturnNull()
        {
            // Given
            int cityId = 12;

            // When
            var Retrived = svc.GetCity(cityId);

            // Then
            Assert.Null(Retrived);
        }// 
    }//Class
}//Namespace