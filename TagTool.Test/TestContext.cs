using TagTool.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace TagTool.Test
{
    class TestContext
    {
        public DataContext GetContext(string DBName)
        {
            /* Test DB Connection */
            DbContextOptionsBuilder<DataContext> optionsBuilder = 
            new DbContextOptionsBuilder<DataContext>();
            /* When unit testing adjust the connection below to fit unit tests */
            optionsBuilder
            // .UseMySql(
            //     "server=localhost;port=3306;user=admin;password=KangarooOnTheRun;database=tagtool_data;");
            .UseSqlite("Filename=" + DBName + ".db");
            return new DataContext(optionsBuilder.Options);
        }
    }
}
