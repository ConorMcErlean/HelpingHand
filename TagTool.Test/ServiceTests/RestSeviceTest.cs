using System;
using Xunit;
using TagTool.Data.Services;
using TagTool.Data.Models;
using System.Threading.Tasks;
using System.Net.Http;

namespace TagTool.Test
{
    public class RestServiceTest
    {
        /* Service Being Tested */
        private readonly IRestService svc;
        /* Dependencies */
        private readonly HttpClient client;

        public RestServiceTest()
        {
            // general arrangement
            client = new HttpClient();
            client.BaseAddress =  new Uri("https://api.what3words.com/v3/");
            svc = new RestService(client);
         
        }// Constructor

        
        [Fact]
        public async void GetWhat3WordsAsync_WhereLatitudeOutsideAPIRange_ShouldThrowException()
        {
            // Given
            /* Range of W3W API is -90 to 90 for both Latitude and longitude */


            // When
  

            // Then
            Task act1() => svc.GetWhat3WordsAsync(-100.00, 45.000);
            Task act2() => svc.GetWhat3WordsAsync(100.00, -45.000);

            await Assert.ThrowsAsync<System.Exception>(act1);
            await Assert.ThrowsAsync<System.Exception>(act2);
        }

        [Fact]
        public async void GetWhat3WordsAsync_WhereLongitudeOutsideAPIRange_ShouldThrowException()
        {
            // Given
            /* Range of W3W API is -90 to 90 for both Latitude and longitude */


            // When


            // Then
            Task act1() => svc.GetWhat3WordsAsync(45, -100.000);
            Task act2() => svc.GetWhat3WordsAsync(45.00, -145.000);

            await Assert.ThrowsAsync<System.Exception>(act1);
            await Assert.ThrowsAsync<System.Exception>(act2);
        }

            [Fact]
        public async void GetWhat3WordsAsync_WhereWithinRange_ReturnShouldBeTypeW3WResponse()
        {
            // Given
            /* Range of W3W API is -90 to 90 for both Latitude and longitude */

            // When
            var Return = await svc.GetWhat3WordsAsync(45.00, 45.000);
  
            // Then
            Assert.NotNull(Return);
            Assert.IsType<What3WordsResponse>(Return);
        }


    }// Class
}// Namespace
