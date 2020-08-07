using TagTool.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TagTool.Data.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        { }
        
        /* The DbSets generate the tables in the Database */
        public DbSet<City> Cities {get; set;}
        public DbSet<Report> Reports {get; set;}
        public DbSet<User> Users {get; set;}

        /* Method to clear and prepare the database */
        public void Initialise()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}