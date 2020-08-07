using System;
using Xunit;
using TagTool.Data.Services;
using TagTool.Data.Repositories;
using TagTool.Data.Models;
using System.Linq;


namespace TagTool.Test
{
    public class AccountServiceTest
    {
        /* Service Being Tested */
        private readonly IAccountService _svc;
        /* Dependencies */
        City city = new City { CityID = 0, Name = "City" };
        /* Database for checks on database */
        private readonly DataContext _db;

        public AccountServiceTest()
        {
            // general arrangement
            _db = new TestContext().GetContext("AccountTest");
            _svc = new AccountService(_db);
            
            // ensure database is empty before each test
            _db.Initialise();
        }


        /* Register Account Tests */

        [Fact]
        public void RegisterAccount_IfAllFieldsFull_ReturnShouldBeTypeUser()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";

            // When
            var Created = _svc.Register(email, Name, role, password, city);

            // Then
            Assert.NotNull(Created);
            Assert.IsType<User>(Created);
        }

        [Fact]
        public void RegisterAccount_IfAllFieldsFull_CreatedShouldHaveUserID()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";


            // When
            var Created = _svc.Register(email, Name, role, password, city);

            // Then
            Assert.True(Created.UserID >= 0);
        }

        [Fact]
        public void RegisterAccount_IfAllFieldsFull_CreatedShouldHavePassedParameters()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";

            // When
            var Created = _svc.Register(email, Name, role, password, city);


            // Then
            Assert.Equal(email, Created.EmailAddress);
            Assert.Equal(Name, Created.Name);
            Assert.Equal(role, Created.Role);
            Assert.NotNull(Created.HashedPassword);
            Assert.Equal(city, Created.City);
        }

        [Fact]
        public void RegisterAccount_IfAllFieldsFull_UserShouldAddToDataBase()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";

            // When
            var Created = _svc.Register(email, Name, role, password, city);
            var Retrieved = _db.Users.FirstOrDefault(u => u.UserID == Created.UserID);


            // Then
            Assert.Equal(Created.EmailAddress, Retrieved.EmailAddress);
            Assert.Equal(Created.Name, Retrieved.Name);
            Assert.Equal(Created.Role, Retrieved.Role);
            Assert.Equal(Created.HashedPassword, Retrieved.HashedPassword);
        }

        [Fact]
        public void RegisterAccount_IfEmailExists_ReturnShouldBeNull()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";

            // When
            var Existing = _svc.Register(email, "Exisiting", Role.Admin, "password", city);
            var Created = _svc.Register(email, Name, role, password, city);

            // Then
            Assert.Null(Created);
        }

        [Fact]
        public void RegisterAccount_IfEmailExists_ShouldNotAddToDatabase()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";

            // When
            var Existing = _svc.Register(email, "Exisiting", Role.Admin, "password", city);
            var Created = _svc.Register(email, Name, role, password, city);
            var Accounts = _db.Users.Select(u => u.EmailAddress == email);

            // Then
            Assert.Equal(1, Accounts.Count());
        }


        [Fact]
        public void RegisterAccount_RegisteringMultiple_ShouldNotAffectCreation()
        {
            // Given
            String email1 = "test1@test.co.uk";
            String email2 = "test2@test.co.uk";
            String email3 = "test3@test.co.uk";
            String Name1 = "TestName1";
            String Name2 = "TestName2";
            String Name3 = "TestName3";
            Role role1 = Role.User;
            Role role2 = Role.Admin;
            Role role3 = Role.User;
            String password1 = "TestPass1";
            String password2 = "TestPass2";
            String password3 = "TestPass3";
            City city1 = new City { Name = "City1", CityID = 1 };
            City city2 = new City { Name = "City2", CityID = 2 };

            // When
            var Created1 = _svc.Register(email1, Name1, role1, password1, city1);
            var Created2 = _svc.Register(email2, Name2, role2, password2, city2);
            var Created3 = _svc.Register(email3, Name3, role3, password3, city1);

            // Then
            Assert.NotNull(Created1);
            Assert.NotNull(Created2);
            Assert.NotNull(Created3);
        }

        /* Get Account By Email Tests */

        [Fact]
        public void GetAccountByEmail_ValidEmail_ShouldReturnTypeUser()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrived = _svc.GetAccountByEmail(email);

            // Then
            Assert.IsType<User>(Retrived);
        }

        [Fact]
        public void GetAccountByEmail_ValidEmail_ShouldReturnRequestedUser()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrived = _svc.GetAccountByEmail(email);

            // Then
            Assert.Equal(Created, Retrived);
        }

        [Fact]
        public void GetAccountByEmail_InValidEmail_ShouldReturnNull()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrived = _svc.GetAccountByEmail("test@testx.co.uk");

            // Then
            Assert.Null(Retrived);
        }

        [Fact]
        public void GetAccountByEmail_NoAccountsExist_ShouldReturnNull()
        {
            // Given

            // When
            var Retrived = _svc.GetAccountByEmail("email@email.co.uk");

            // Then
            Assert.Null(Retrived);
        }

        /* Close Account Tests */

        [Fact]
        public void CloseAccount_ValidUser_ShouldReturnTrue()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Closed = _svc.CloseAccount(Created);

            // Then
            Assert.True(Closed);
        }

        [Fact]
        public void CloseAccount_ValidUser_ShouldRemoveUserFromDatabase()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Closed = _svc.CloseAccount(Created);
            var Retrieved = _db.Users.FirstOrDefault(u => u.EmailAddress == email);

            // Then
            Assert.Null(Retrieved);
        }

        [Fact]
        public void CloseAccount_InValidUser_ShouldReturnFalse()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);
            User invalid = null;

            // When
            var Closed = _svc.CloseAccount(invalid);

            // Then
            Assert.False(Closed);
        }

        /* Authenticate Tests */

        [Fact]
        public void Authenticate_ValidEmail_ValidPassword_ShouldReturnTypeUser()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrieved = _svc.Authenticate(email, password);

            // Then
            Assert.IsType<User>(Retrieved);
        }

        [Fact]
        public void Authenticate_ValidEmail_ValidPassword_ShouldReturnUser()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrieved = _svc.Authenticate(email, password);

            // Then
            Assert.Equal(Created.EmailAddress, Retrieved.EmailAddress);
            Assert.Equal(Created.Name, Retrieved.Name);
            Assert.Equal(Created.Role, Retrieved.Role);
            Assert.Equal(Created.HashedPassword, Retrieved.HashedPassword);
            Assert.Equal(Created.CityID, Retrieved.CityID);
        }

        [Fact]
        public void Authenticate_ValidEmail_InValidPassword_ShouldReturnNull()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrieved = _svc.Authenticate(email, "TesPass");

            // Then
            Assert.Null(Retrieved);
        }

        [Fact]
        public void Authenticate_InValidEmail_ValidPassword_ShouldReturnNull()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrieved = _svc.Authenticate("tes@test.co.uk", password);

            // Then
            Assert.Null(Retrieved);
        }

        [Fact]
        public void Authenticate_InValidEmail_InValidPassword_ShouldReturnNull()
        {
            // Given
            String email = "test@test.co.uk";
            String Name = "TestName";
            Role role = Role.User;
            String password = "TestPass";
            var Created = _svc.Register(email, Name, role, password, city);

            // When
            var Retrieved = _svc.Authenticate("other@other.com", "password");

            // Then
            Assert.Null(Retrieved);
        }

        [Fact]
        public void Authenticate_NoAccounts_ShouldReturnNull()
        {
            // Given
            String email = "test@test.co.uk";
            String password = "TestPass";

            // When
            var Retrieved = _svc.Authenticate(email, password);

            // Then
            Assert.Null(Retrieved);
        }

        [Fact]
        public void ViewAll_NoAccounts_ShouldReturnEmptyList()
        {
            // Given
            string orderby = null;

            // When
            var Retrieved = _svc.ViewAllAccounts(orderby);

            // Then
            Assert.Empty(Retrieved);
        }

        [Fact]
        public void ViewAll_Accounts_ShouldReturnListOfCorrectLength()
        {
            // Given
            string orderby = null;
            var user1 = _svc.Register("test@test.co.uk", "TestName", Role.User, "TestPass", city);
            var user2 = _svc.Register("hello@test.co.uk", "Bob", Role.User, "BobsPassword", city);
            var user3 = _svc.Register("admin@test.co.uk", "Sarah", Role.Admin, "AdminsSecurePassword", city);
            var user4 = _svc.Register("user@test.co.uk", "David", Role.User, "UsersPassword", city);

            // When
            var Retrieved = _svc.ViewAllAccounts(orderby);

            // Then
            Assert.True(Retrieved.Count == 4);
        }

        [Fact]
        public void ViewAll_Accounts_ShouldReturnCorrectOrder()
        {
            // Given
            string orderby = null;
            var user1 = _svc.Register("test@test.co.uk", "TestName", Role.User, "TestPass", city);
            var user2 = _svc.Register("hello@test.co.uk", "Bob", Role.User, "BobsPassword", new City { Name ="ACity", CityID = 2});
            var user3 = _svc.Register("admin@test.co.uk", "Sarah", Role.Admin, "AdminsSecurePassword", new City { Name = "BCity", CityID = 3 });

            // When
            orderby = "Name";
            var NameOrderedList = _svc.ViewAllAccounts(orderby);
            var Nameexpected = _db.Users.OrderBy(u => u.Name).ToList();

            orderby = "Email";
            var EmailOrderedList = _svc.ViewAllAccounts(orderby);
            var Emailexpected = _db.Users.OrderBy(u => u.EmailAddress).ToList();

            orderby = "Role";
            var RoleOrderedList = _svc.ViewAllAccounts(orderby);
            var Roleexpected = _db.Users.OrderBy(u => u.Role).ToList();

            orderby = "City";
            var CityOrderedList = _svc.ViewAllAccounts(orderby);
            var Cityexpected = _db.Users.OrderBy(u => u.City).ToList();
            

            // Then
            for (int i = 0; i < NameOrderedList.Count(); i++)
            {
                Assert.Equal(Nameexpected[i].UserID, NameOrderedList[i].UserID);
                Assert.Equal(Emailexpected[i].UserID, EmailOrderedList[i].UserID);
                Assert.Equal(Roleexpected[i].UserID, RoleOrderedList[i].UserID);
                Assert.Equal(Cityexpected[i].UserID, CityOrderedList[i].UserID);
            }
        }
    }
}
