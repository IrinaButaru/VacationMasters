using System.Collections.Generic;
using NUnit.Framework;
using VacationMasters.Essentials;
using VacationMasters.UnitTests.Infrastructure;
using VacationMasters.UserManagement;
using VacationMasters.Wrappers;
using VacationMasters.PackageManagement;
using System.Linq;
using System;
using System.Reflection;

namespace VacationMasters.UnitTests.DatabaseTests
{
    class DatabaseCommandsTests
    {
        private IDbWrapper _dbWrapper;
        private IUserManager _userManagement;
        private PackageManager _packageManager;

        [SetUp]
        public void SetUp()
        {
            _dbWrapper = new DbWrapper();
            _userManagement = new UserManager(_dbWrapper);
            _packageManager = new PackageManager(_dbWrapper);
        }



        private User CreateRandomUser()
        {
            var user = new User(CreateRandom.String(), CreateRandom.String(), CreateRandom.String(),
                CreateRandom.String(), CreateRandom.String(), false, CreateRandom.String(), CreateRandom.String());

            return user;
        }

        [Test]
        public void LMDShouldNotThrow()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();

            Assert.DoesNotThrow(() => _userManagement.AddUser(user, password));
            Assert.DoesNotThrow(() => _userManagement.RemoveUser(user.UserName));
        }

        [Test]
        public void AddUserWithPreferencesAndGroupsShouldNotThrow()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();
            var preferences = new List<string>();
            preferences.Add("Germania");
            preferences.Add("Citibreak");
            var groups = new List<string> { "Grupul iubitorilor de mare" };
            Assert.DoesNotThrow(() => _userManagement.AddUser(user, password, preferences, groups));
            Assert.DoesNotThrow(() => _userManagement.RemoveUser(user.UserName));
        }

        [Test]
        public void GetUserShouldNotThrow()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();

            _userManagement.AddUser(user, password);

            var localUser = _userManagement.GetUser(user.UserName);

            Assert.That(user.UserName == localUser.UserName);

            _userManagement.RemoveUser(user.UserName);
        }

        [Test]
        public void BanUserShouldNotThrow()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();
            user.Banned = true;

            _userManagement.AddUser(user, password);

            _userManagement.BanUser(user.UserName);
            var localUser = _userManagement.GetUser(user.UserName);
            Assert.That(localUser.Banned == user.Banned);

            _userManagement.RemoveUser(user.UserName);
        }

        [Test]
        public void UnbanUserShouldNotThrow()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();
            user.Banned = false;

            _userManagement.AddUser(user, password);

            _userManagement.UnbanUser(user.UserName);
            var localUser = _userManagement.GetUser(user.UserName);
            Assert.That(localUser.Banned == user.Banned);

            _userManagement.RemoveUser(user.UserName);
        }

        [Test]
        public void MultipleUsersCanLoginAtTheSameTime()
        {
            var password1 = CreateRandom.String();
            var user1 = CreateRandomUser();

            var password2 = CreateRandom.String();
            var user2 = CreateRandomUser();

            _userManagement.AddUser(user1, password1);
            _userManagement.AddUser(user2, password2);

            Assert.DoesNotThrow(() => _userManagement.Login(user1.UserName, password1));
            Assert.DoesNotThrow(() => _userManagement.Login(user2.UserName, password2));

            Assert.IsTrue(UserManager.CurrentUser.UserName == user1.UserName);
            Assert.IsFalse(UserManager.CurrentUser.UserName == user2.UserName);

            _userManagement.RemoveUser(user1.UserName);
            _userManagement.RemoveUser(user2.UserName);
        }

        [Test]
        public void ValidUserWithPasswordPassCredentialChecking()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();

            _userManagement.AddUser(user, password);

            Assert.DoesNotThrow(() => _userManagement.Login(user.UserName, password));
        
            Assert.NotNull(UserManager.CurrentUser);

            _userManagement.RemoveUser(user.UserName);
        }

        [Test]
        public void ValidUserWithWrongPasswordFailsCredentialChecking()
        {
            var password = CreateRandom.String();
            var password1 = CreateRandom.String();
            var user = CreateRandomUser();

            _userManagement.AddUser(user, password);

            _userManagement.Login(user.UserName, password1);
      
          //  Assert.IsNull(UserManager.CurrentUser);
            
            _userManagement.RemoveUser(user.UserName);
        }

        [Test]
        public void NonExistingUserTriesToLogin()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();

            _userManagement.Login(user.UserName, password);

         //  Assert.IsNull(UserManager.CurrentUser);
        }

        [Test]
        public void UserCanChangeHisPassword()
        {
            var password = CreateRandom.String();
            var user = CreateRandomUser();
            var newPassword = CreateRandom.String();

            _userManagement.AddUser(user,password);

            Assert.DoesNotThrow(() => _userManagement.ChangePassword(user.UserName, newPassword));
            Assert.True(_userManagement.CheckCredentials(user.UserName, newPassword));

            _userManagement.RemoveUser(user.UserName);
        }


        public Package CreateTestPackage()
        {
            var package = new Package("testpack", "croaziera", "chestii", "vapor", 7000.0, 2.0, 4.0, new DateTime(2015, 7, 16), new DateTime(2015, 7, 26), null);
            return package;
        }

        [Test]
        public void InsertingNewPackageShouldNotThrow()
        {
            var pack = CreateTestPackage();

            Assert.DoesNotThrow(() => _packageManager.AddPackage(pack));
            var idPack = _dbWrapper.QueryValue<int>(string.Format("Select Id From Packages Where Name='{0}'", pack.Name));
            pack.ID = idPack;
            Assert.DoesNotThrow(() => _packageManager.RemovePackage(pack));
        }

        [Test]
        public void GivenNameSelectPackageByName()
        {
            var pack = CreateTestPackage();

            _packageManager.AddPackage(pack);

            var list = _dbWrapper.GetPackagesByName(pack.Name);
            Assert.That(list.FirstOrDefault().Name == pack.Name);
            var idPack = _dbWrapper.QueryValue<int>(string.Format("Select Id From Packages Where Name='{0}'", pack.Name));
            pack.ID = idPack;

            _packageManager.RemovePackage(pack);

        }

        [Test]
        public void GivenPriceSelectPackageByPrice()
        {
            var pack = CreateTestPackage();

            _packageManager.AddPackage(pack);

            var list = _dbWrapper.GetPackagesByPrice(1000.0, 8000.0);
            foreach (Package item in list)
            {
                if (item.Name.Equals("testpack"))
                    Assert.That(item.Price == pack.Price);
            }
            var idPack = _dbWrapper.QueryValue<int>(string.Format("Select Id From Packages Where Name='{0}'", pack.Name));
            pack.ID = idPack;

            _packageManager.RemovePackage(pack);
        }

        [Test]
        public void GivenDateSelectPackageByDate()
        {
            var pack = CreateTestPackage();

            _packageManager.AddPackage(pack);

            var list = _dbWrapper.GetPackagesByDate(new DateTime(2015, 7, 16), new DateTime(2015, 7, 26));
            foreach (Package item in list)
            {
                if (item.Name.Equals("testpack"))
                    Assert.That(item.BeginDate == pack.BeginDate && item.EndDate == pack.EndDate);
            }
            var idPack = _dbWrapper.QueryValue<int>(string.Format("Select Id From Packages Where Name='{0}'", pack.Name));
            pack.ID = idPack;

            _packageManager.RemovePackage(pack);
        }

        [Test]
        public void GivenTypeSelectPackageByType()
        {
            var pack = CreateTestPackage();

            _packageManager.AddPackage(pack);
            var list = _dbWrapper.getPackagesByType("croaziera");
            foreach (Package item in list)
            {
                if (item.Name.Equals("testpack"))
                    Assert.That(item.Type == pack.Type);
            }
            var idPack = _dbWrapper.QueryValue<int>(string.Format("Select Id From Packages Where Name='{0}'", pack.Name));
            pack.ID = idPack;
            _packageManager.RemovePackage(pack);
        }

        [Test]
        public void ReserveAndCancelShouldNotThrowAndRemainNoOrder()
        {
            var pack = CreateTestPackage();
            var password = CreateRandom.String();
            var user = CreateRandomUser();
            var preferences = new List<string>();
            preferences.Add("Germania"); preferences.Add("Citibreak");
            var groups = new List<string> { "Grupul iubitorilor de mare" };
            _userManagement.AddUser(user, password, preferences, groups);
            _packageManager.AddPackage(pack);
            var idPack = _dbWrapper.QueryValue<int>(string.Format("Select Id From Packages Where Name='{0}'", pack.Name));
            Assert.DoesNotThrow(() => _packageManager.ReservePackage(user.UserName, DateTime.Now, pack.Price, idPack));
            var orderId = _packageManager.CheckIfUserHasOrderedThePackage(idPack, user.UserName);
            Assert.DoesNotThrow(() => _packageManager.CancelReservation(idPack, orderId));
            orderId = _packageManager.CheckIfUserHasOrderedThePackage(idPack, user.UserName);
            Assert.AreEqual(orderId, 0);
            pack.ID = idPack;
            _packageManager.RemovePackage(pack);
            _userManagement.RemoveUser(user.UserName);
        }

        [Test]
        public void UpdateRatingSuccess()
        {
            var pack = CreateTestPackage();
            var password = CreateRandom.String();
            var user = CreateRandomUser();
            var preferences = new List<string>();
            preferences.Add("Germania"); preferences.Add("Citibreak");
            var groups = new List<string> { "Grupul iubitorilor de mare" };
            _userManagement.AddUser(user, password, preferences, groups);
            _packageManager.AddPackage(pack);
            var idPack = _dbWrapper.QueryValue<int>(string.Format("Select Id From Packages Where Name='{0}'", pack.Name));
            _packageManager.ReservePackage(user.UserName, DateTime.Now, pack.Price, idPack);
            var orderId = _packageManager.CheckIfUserHasOrderedThePackage(idPack, user.UserName);
            var random = new Random();
            var int1 = random.Next(1, 5);
            var int2 = random.Next(1, 5);
            var int3 = random.Next(1, 5);
            double average = ((double)int1 + (double)int2 + (double)int3)/3;
            Assert.DoesNotThrow(() => _packageManager.UpdateRating(idPack, int1, orderId));
            Assert.DoesNotThrow(() => _packageManager.UpdateRating(idPack, int2, orderId));
            Assert.DoesNotThrow(() => _packageManager.UpdateRating(idPack, int3, orderId));
            var rating = _dbWrapper.QueryValue<double>(string.Format("Select Rating From Packages Where Id={0}", idPack));
            Assert.AreEqual(Math.Round(average,3),Math.Round(rating,3));
            _packageManager.CancelReservation(idPack, orderId);
            pack.ID = idPack;
            _packageManager.RemovePackage(pack);
            _userManagement.RemoveUser(user.UserName);
        }
        [Test]
        public void DummyTest()
        {

        }


    }
}
