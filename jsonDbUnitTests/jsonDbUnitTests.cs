using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Reflection;
using jsonDb;

namespace jsonDbUnitTests
{
    [TestClass]
    public class jsonDbUnitTests
    {
        const string LASTNAME      = "Torres";
        const string FIRSTNAME     = "Frederic";
        static DateTime BIRTHDATE  = new DateTime(1964, 12, 11);
        const bool US_CITIZEN      = false;
        const string SSN           = null;
        const double HEIGHT        = 1.8;

        [TestMethod]
        public void SerializeDeserializePoco()
        {
            var poco1 = new Poco1() { 
                LastName  = LASTNAME,
                FirstName = FIRSTNAME,   
                BirthDate = BIRTHDATE,
                USCitizen = US_CITIZEN,
                Height    = HEIGHT,
                SSN       = SSN,
            };
         
            var json  = poco1.Serialize();
            var poco2 = JDbObject.Deserialize<Poco1>(json);
            ValidatePoco1(poco1);
        }

        [TestMethod]
        public void DeserializePoco1EmbdedSample()
        {
            var json  = DS.Resources.GetTextResource("Poco1.json", Assembly.GetExecutingAssembly());
            var poco1 = JDbObject.Deserialize<Poco1>(json);
            ValidatePoco1(poco1);
            var json2 = JDbObject.Serialize<Poco1>(poco1);
        }

        [TestMethod]
        public void FileSystemStore_SaveAndLoad()
        {
            var poco1 = new Poco1() {
                LastName  = LASTNAME,
                FirstName = FIRSTNAME,
                BirthDate = BIRTHDATE,
                USCitizen = US_CITIZEN,
                Height    = HEIGHT,
                SSN       = SSN,
            };
            poco1.__metadata.Store = new FileSystemStore();
            poco1.Save();

            var fs = poco1.__metadata.Store as FileSystemStore;
            var poco2 = poco1.__metadata.Store.Load<Poco1>(poco1.__metadata.Id);
           // var poco2 = fs.Load<Poco1>(poco1.__metadata.Id);
            ValidatePoco1(poco2);
        }

        private static void ValidatePoco1(Poco1 poco)
        {
            DS.Assert.ValueTypeProperties(poco,
                                                new
                                                {
                                                    LastName  = LASTNAME,
                                                    FirstName = FIRSTNAME,
                                                    BirthDate = BIRTHDATE,
                                                    USCitizen = US_CITIZEN,
                                                    Height    = HEIGHT,
                                                    SSN       = SSN,
                                                }
                                          );
        }

        private void AssertAuthorizationSystem() {

        }

        [TestMethod]
        public void SerializeWorld()
        {
            const string ADMINISTRATOR_GROUP_NAME = "Administrators";
            const string USER_GROUP_NAME          = "Users";

            // Initialize an Authorization System
            var  w = new AuthorizationSystem();

            // Defines the  3 types of roles we want to support
            w.VMRoles.Add(DS.List("VM.Provision", "VM.TurnOff", "VM.TurnOn", "VM.Delete", "VM.InstallTools"));
            w.TemplateRoles.Add(DS.List("Template.Create", "Template.Delete", "Template.Edit" ));
            w.UserRoles.Add(DS.List("User.Create", "User.Delete", "User.Edit" ));

            // Create 2 groups and assign them roles
            w.Groups.Add(new Group() { Name = ADMINISTRATOR_GROUP_NAME, Roles = w.GetAllRoles() } );
            w.Groups.Add(new Group() { Name = USER_GROUP_NAME, Roles = w.VMRoles} );
            
            // Create 3 users and assign them 1 group
            w.Users.Add(new User().Create("fCompany", "JoeDoe").AddGroups(
                DS.List(w.Groups.GetByName(ADMINISTRATOR_GROUP_NAME), 
                w.Groups.GetByName(USER_GROUP_NAME)))
            );
            w.Users.Add(new User().Create("fCompany", "JaneDoe").AddGroup(w.Groups.GetByName(USER_GROUP_NAME)));
            w.Users.Add(new User().Create("fCompany", "RodgerRabbit").AddGroup(w.Groups.GetByName(USER_GROUP_NAME)));
    
            w.__metadata.Store = new FileSystemStore();
            w.Save();

            var fs = w.__metadata.Store as FileSystemStore;
            var w2 = w.__metadata.Store.Load<AuthorizationSystem>(w.__metadata.Id);

            
            Assert.IsTrue(w.Users.GetByName("RodgerRabbit").IsAuthorized(w.GetAllRoles().GetByName("VM.Provision")));
            Assert.IsTrue(w.Users.GetByName("RodgerRabbit").IsAuthorized(w.VMRoles));

            Assert.IsFalse(w.Users.GetByName("RodgerRabbit").IsAuthorized(w.GetAllRoles().GetByName("Template.Edit")));
            Assert.IsFalse(w.Users.GetByName("RodgerRabbit").IsAuthorized(w.GetAllRoles()));

        }
    }
}
