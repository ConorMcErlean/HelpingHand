using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TagTool.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using TagTool.Data.Services;
using TagTool.Data.Seeders;
using System.Net.Http.Headers;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
namespace TagTool.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            /* ********** Dependency Injection ************** */
            services.AddDbContext<DataContext>(options => 
                options.UseMySql(
                    Configuration.GetConnectionString("RemoteDB")
                    ));
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


            /* **** Authentication Setup **** */
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.AccessDeniedPath = "/User/ErrorNotAuthorised";
                    options.LoginPath = "/User/ErrorNotAuthenticated";
                });

            /* **** Hangfire Storage Setup **** */

            services.AddHangfire(config => 
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseDefaultTypeSerializer()
                        .UseMemoryStorage()
                );
            services.AddHangfireServer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            IBackgroundJobClient backgroundJobClient,
            IRecurringJobManager recurringJobManager,
            IServiceProvider serviceProvider
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            /* Hangfire Server Setup */
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new [] { new MyAuthorizationFilter() }
            });
            backgroundJobClient.Enqueue(() => Console.WriteLine("Hangfire online."));
            backgroundJobClient.Enqueue(() => serviceProvider.GetService<ISeeder>().Seed());
            recurringJobManager.AddOrUpdate(
                "Auto-Delete",
                () => serviceProvider.GetService<IDeleteService>().CleanRecordsASync(),
                Cron.Hourly
            );
            recurringJobManager.AddOrUpdate(
                "Reset",
                () => serviceProvider.GetService<ISeeder>().Seed(),
                "*/30 * * * *"
            );
            
            
        }
    }
}
