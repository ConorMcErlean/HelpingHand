using TagTool.Data.Services;
using TagTool.Data.Seeders;
using TagTool.Data.Repositories;
using TagTool.Data.Models;
using System.Linq;
using Xunit;

namespace TagTool.Test
{
    public class SeederTest
    {
        /* Service Being Tested */
        private readonly ISeeder _svc;
        
        /* Dependencies */
        

        /* Database for checks on database */
        private readonly DataContext _db;

        public SeederTest()
        {
            // general arrangement
            _db = new TestContext().GetContext("SeedTest");
            _svc = new Seeder(_db);
            
        }


        /* Test Cases */

        [Fact]
        public void Seeder_ShouldSeed_Cities()
        {
            // Given
            _db.Initialise();

            // When
            _svc.Seed();

            // Then

            // We expect 3 Cites, Belfast, Derry, Non-City
            var Cities = _db.Cities.ToList();
            Assert.Equal(3, Cities.Count);

            // We Expect a City with the Name Belfast & a CityID
            var Belfast = _db.Cities
            .FirstOrDefault(c => c.Name == "Belfast, City of Belfast");
            Assert.NotNull(Belfast);
            Assert.True(Belfast.CityID > 0);

            // We Expect a City with the Name Derry & a CityID
            var Derry = _db.Cities
            .FirstOrDefault(c => c.Name == "Derry, Derry City and Strabane");
            Assert.NotNull(Derry);
            Assert.True(Derry.CityID > 0);
            Assert.NotEqual(Derry.CityID, Belfast.CityID);

            // We Expect a 'non' City with a CityID
            var Other = _db.Cities
            .FirstOrDefault(c => c.Name == "Non-City Area");
            Assert.NotNull(Other);
            Assert.True(Other.CityID > 0);
            Assert.NotEqual(Other.CityID, Belfast.CityID);
            Assert.NotEqual(Other.CityID, Derry.CityID);
        }

        [Fact]
        public void Seeder_ShouldSeed_Users()
        {
            // Given
            _db.Initialise();

            // When
            _svc.Seed();

            // Then

            // We Expect 4 Users
            var Users = _db.Users.ToList();
            Assert.Equal(4, Users.Count);

            // We Expect an Admin User
            var Admin = _db.Users
            .FirstOrDefault(u => u.EmailAddress == "Admin@test.com");
            Assert.NotNull(Admin);
            Assert.True(Admin.Name == "Admin");
            Assert.True(Admin.Role == Role.Admin);
            Assert.True(Admin.HashedPassword != "Password");

            // We Expect a User User
            var User = _db.Users
            .FirstOrDefault(u => u.EmailAddress == "User@test.com");
            Assert.NotNull(User);
            Assert.True(User.Name == "User");
            Assert.True(User.Role == Role.User);
            Assert.True(User.HashedPassword != "Password");

            // We Expect a Developer User
            var Dev = _db.Users
            .FirstOrDefault(u => u.EmailAddress == "Conor@cmcerlean.com");
            Assert.NotNull(Dev);
            Assert.True(Dev.Name == "Conor");
            Assert.True(Dev.Role == Role.Admin);
            Assert.True(Dev.HashedPassword != "80lUJy#&EIRovFjdQTPEx#RX2@nVz!@Hi6JV*N*X01RpQyMb19O%");

            // We Expect a Test Account
            var Test = _db.Users
            .FirstOrDefault(u => u.EmailAddress == "9h5an.test@inbox.testmail.app");
            Assert.NotNull(Test);
            Assert.True(Test.Name == "Example");
            Assert.True(Test.Role == Role.User);
            Assert.True(Test.HashedPassword != "ce@gIPP!Z!XESF2#b8sCaIKfi8maEHV@j7lLUfzegWX&2cBs&F&#");
        }

        [Fact]
        public void Seeder_ShouldSeed_Reports()
        {
            // Given
            _db.Initialise();

            // When
            _svc.Seed();

            // Then

            // We Expect 7 Reports
            Assert.Equal(7, _db.Reports.ToList().Count);

            // We Expect 4 in Belfast, 3 in Derry
            var Belfast = _db.Cities
            .First(c => c.Name == "Belfast, City of Belfast");
            var Derry = _db.Cities
            .First(c => c.Name == "Derry, Derry City and Strabane");

            Assert.Equal(4, _db.Reports.Where(r => r.CityID == Belfast.CityID)
            .ToList().Count);
            Assert.Equal(3, _db.Reports.Where(r => r.CityID == Derry.CityID)
            .ToList().Count);
        }

    }
}