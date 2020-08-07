using Microsoft.Extensions.DependencyInjection;
using TagTool.Data.Repositories;
using TagTool.Data.Services;
using TagTool.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System;

namespace TagTool.Test
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => 
                options.UseSqlite("Filename=IntegrationTests.db"));
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IDeleteService, DeleteService>();
            services.AddTransient<ISeeder, Seeder>();
            services.AddHttpClient<IRestService, RestService>(client => {
                client.BaseAddress = new Uri("https://api.what3words.com/v3/");
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }
    }
}