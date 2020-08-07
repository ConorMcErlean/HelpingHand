using System;
using TagTool.Data.Secrets;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TagTool.Data.Models;
using TagTool.Data.Repositories;



/* 
API consummer using advice from:
https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
*/

namespace TagTool.Data.Services
{
    public class RestService : IRestService
    {
        // Required by the rest service
        private readonly HttpClient client;
        private string baseQuery;

        public RestService(HttpClient client)
        {
            this.client = client;
            this.baseQuery = client.BaseAddress + "convert-to-3wa?key="+ GetKey.What3WordsAPIKey() + 
            "&language=en&format=json";
        }

        // The API call using System.net
        public async Task<What3WordsResponse> GetWhat3WordsAsync(double lat, double lng)
        {
            if ((lat > 90 || lat <-90) || (lng > 90 || lng < -90)) { throw new Exception("Outside API lat/lng bounds of -90 to 90."); }
            What3WordsResponse W3Wresponse = null;
            HttpResponseMessage response = await client.GetAsync(
                this.baseQuery + "&coordinates=" + 
                lat.ToString("00.000000") + "," +
                lng.ToString("00.000000") 
            );
            if (response.IsSuccessStatusCode)
            {
                W3Wresponse = await response.Content.ReadAsAsync<What3WordsResponse>();
            }
            else
            {
                throw new Exception(response.ReasonPhrase);
            }
            return W3Wresponse;
        }
    }// RestClientClass
}