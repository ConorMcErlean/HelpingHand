using TagTool.Data.Models;
using System.Collections.Generic;

namespace TagTool.Data.Services
{
    /* 
    The abstraction of AccountSevice, allows the service to be injected through 
    dependency injection.
    */
    public interface ICityService
    {
        /* Create Methods */
            
            /* Method to create a city zone. */
            City CreateCity(string Name);


        /* Get Methods */

            /* Method to get a city by it's CityID Property. */
            City GetCity(int CityID);

            /* Method to get a the default non city value. */
            public City GetOutsideCity();
        
            /* Method to get a list of all cities. */
            IList<City> GetCities();
    
    }

}