using TagTool.Data.Services;
using TagTool.Data.Models;
using TagTool.Data.Repositories;
using System;

namespace TagTool.Data.Seeders
{
    /* Seeder class to generate false data */
    public class Seeder : ISeeder
    {
        /* Dependencies */
        private readonly DataContext _db;
        public Seeder(DataContext db)
        {
            _db = db;
        }
        /* Method to seed the database */
        public void Seed(){
            
            var _ASvc = new AccountService(_db);
            var _CSvc = new CityService(_db);
            
            // Initialise the database.
            _db.Initialise();
            // Create a city & an organisation.
            var City = _CSvc.CreateCity("Belfast, City of Belfast");
            var City2 = _CSvc.CreateCity("Derry, Derry City and Strabane");
            
            // Register two users.
            _ASvc.Register ("Admin@test.com", "Admin", Role.Admin, "Password",
             City);
            _ASvc.Register ("User@test.com", "User", Role.User, "Password", 
            City2);
            var Developer = _ASvc.Register ("Conor@cmcerlean.com", "Conor", Role.Admin, "80lUJy#&EIRovFjdQTPEx#RX2@nVz!@Hi6JV*N*X01RpQyMb19O%",
            _CSvc.GetOutsideCity());
            var TestAccount = _ASvc.Register ("9h5an.test@inbox.testmail.app", "Example", Role.User, "ce@gIPP!Z!XESF2#b8sCaIKfi8maEHV@j7lLUfzegWX&2cBs&F&#", 
            _CSvc.GetOutsideCity());

            // Register four simple reports for testing.

            var Report1 = new Report
            {
                Latitude = 54.597216,
                Longitude = -5.930420,
                CreatedAt = (DateTime.Now).AddHours(-3),
                AdditionalInfo = "Report from Belfast City Hall",
                Active = true,
                ThreeWordAddress = "twigs.purple.pulled",
                City = City
            };

            var Report2 = new Report
            {
                Latitude = 54.584500,
                Longitude = -5.934398,
                CreatedAt = (DateTime.Now).AddHours(-1.5),
                AdditionalInfo = "Report from Queens University Belfast",
                Active = true,
                ThreeWordAddress = "input.estate.cloud",
                City = City
            };

            var Report3 = new Report
            {
                Latitude = 54.604949,
                Longitude = -5.904914,
                CreatedAt = (DateTime.Now).AddHours(-0.5),
                AdditionalInfo = "Report from Samson & Goliath",
                Active = true,
                ThreeWordAddress = "legs.rivers.beats",
                City = City
            };

            var Report4 = new Report
            {
                //54.598696, -5.927506
                Latitude = 54.598696,
                Longitude = -5.927506,
                CreatedAt = DateTime.Now,
                AdditionalInfo = "Report from Victoria Square Statue",
                Active = true,
                ThreeWordAddress = "silver.snap.castle",
                City = City
            };

            var Report5 = new Report
            {
                Latitude = 54.997646,
                Longitude = -7.319230,
                CreatedAt = (DateTime.Now).AddHours(-6),
                AdditionalInfo = "Report from Derry Guild Hall",
                Active = true,
                ThreeWordAddress = "given.rash.toys",
                City = City2
            };

            var Report6 = new Report
            {
                Latitude = 55.006164, 
                Longitude = -7.323620,
                CreatedAt = (DateTime.Now).AddHours(-12),
                AdditionalInfo = "Report from Magee Campus",
                Active = true,
                ThreeWordAddress = "train.intent.divide",
                City = City2
            };

            var Report7 = new Report
            {
                Latitude = 55.006219,
                Longitude = -7.323624,
                CreatedAt = (DateTime.Now).AddHours(-36),
                AdditionalInfo = "Report from Peace Bridge",
                Active = true,
                ThreeWordAddress = "stone.birds.meal",
                City = City2
            };
            TestAccount.EmailNotifications = true;

            // Add Reports to Database
            _db.Add(Report1);
            _db.Add(Report2);
            _db.Add(Report3);
            _db.Add(Report4);
            _db.Add(Report5);
            _db.Add(Report6);
            _db.Add(Report7);
            _db.SaveChanges();
        }
    } 
}