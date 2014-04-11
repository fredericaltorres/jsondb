using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicSugar;
using System.Reflection;
using jsonDb;

namespace JSonObjectUnitTests
{
    class Poco1 : System.JSON.JSonObject
    {
        public string LastName;
        public string FirstName;
        public DateTime BirthDate;
        public bool USCitizen;
        public double Height;
        public string SSN;
    }
    
    [TestClass]
    public class JSonObject
    {
        const string LASTNAME      = "Torres";
        const string FIRSTNAME     = "Frederic";
        static DateTime BIRTHDATE  = new DateTime(1964, 12, 11);
        const bool US_CITIZEN      = false;
        const string SSN           = null;
        const double HEIGHT        = 1.8;

        [TestMethod]
        public void Poco_Serialize()
        {
            var expectedSerialization = @"{
  'LastName': 'Torres',
  'FirstName': 'Frederic',
  'BirthDate': '1964-12-11T00:00:00',
  'USCitizen': false,
  'Height': 1.8,
  'SSN': null
}".Replace("'",@"""");


            var poco1 = new Poco1() { 
                LastName  = LASTNAME,
                FirstName = FIRSTNAME,   
                BirthDate = BIRTHDATE,
                USCitizen = US_CITIZEN,
                Height    = HEIGHT,
                SSN       = SSN,
            };
            
            Assert.AreEqual(expectedSerialization, poco1.Serialize());
            Assert.AreEqual(expectedSerialization, System.JSON.JSonObject.Serialize(poco1));
            
        }

        [TestMethod]
        public void Deserialize()
        {
            var poco1 = new Poco1()
            {
                LastName = LASTNAME,
                FirstName = FIRSTNAME,
                BirthDate = BIRTHDATE,
                USCitizen = US_CITIZEN,
                Height = HEIGHT,
                SSN = SSN,
            };
            var s = poco1.Serialize();
            var poco2 = System.JSON.JSonObject.Deserialize<Poco1>(s);
            Assert.AreEqual(s, poco2.Serialize());

            var poco3 = Poco1.Deserialize<Poco1>(s);
            Assert.AreEqual(s, poco3.Serialize());
        }
        
        [TestMethod]
        public void ListString_Serialize()
        {
            var l = new List<string>() { "a", "b", "c", "d" };
            
            var expectedSerialization = @"[
  'a',
  'b',
  'c',
  'd'
]".Replace("'", @"""");
            var aa = System.JSON.JSonObject.Serialize(l);
            Assert.AreEqual(expectedSerialization, System.JSON.JSonObject.Serialize(l));

        }


        [TestMethod]
        public void DictionaryString_Serialize()
        {
            var l = new Dictionary<string, int>() { { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 4 }, };

            var expectedSerialization = @"{
  'a': 1,
  'b': 2,
  'c': 3,
  'd': 4
}".Replace("'", @"""");

            var aa = System.JSON.JSonObject.Serialize(l);
            Assert.AreEqual(expectedSerialization, System.JSON.JSonObject.Serialize(l));

        }


    }
}
