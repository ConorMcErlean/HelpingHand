using System.Linq;
using System.Collections.Generic;
using TagTool.Data.Models;
using TagTool.Data.Repositories;
using TagTool.Data.Security;

namespace TagTool.Data.Services
{

    public class CityService : ICityService
    {
    /* Dependencies */
        private readonly DataContext _db;

        /* Constuctor with Constructor DI-injection */
        public CityService(DataContext db)
        {
            _db = db;
        }

         /* Method to create a city zone */
        public City CreateCity(string Name)
        {
            // Ensure city does not already exist.
            var check = _db.Cities.FirstOrDefault(c => c.Name == Name);
            if (check != null) { return null; }

            var City = new City { Name = Name };

            _db.Cities.Add(City);
            _db.SaveChanges();
            return City;
        }// CreateCity

        /* Method to get a city by CityID parameter */
        public City GetCity(int CityID)
        {
            return _db.Cities.FirstOrDefault (c => c.CityID == CityID);
        }
        
        /* Method to get a list of cities */
        public IList<City> GetCities() { return _db.Cities.ToList(); }

        public City GetOutsideCity()
        {
            var NonCity =  _db.Cities.FirstOrDefault (c => c.Name == "Non-City Area");
            if (NonCity == null){
                NonCity = new City { Name = "Non-City Area" };
                // Will only be called by Report Service so no need to save.
                _db.Cities.Add(NonCity);
            }
            return NonCity;
        }
    }
}