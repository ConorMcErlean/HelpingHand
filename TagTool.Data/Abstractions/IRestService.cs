using TagTool.Data.Models;
using System.Threading.Tasks;

namespace TagTool.Data.Services
{
    /* 
    The abstraction of RestSevice, allows the service to be injected through 
    dependency injection.
    */
    public interface IRestService
    {
        /* 
        Method to get a API respone passing a path as a parameter.
        */
        public Task<What3WordsResponse> GetWhat3WordsAsync(double lat, double lng);
    }
}