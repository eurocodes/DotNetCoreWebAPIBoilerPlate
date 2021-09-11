using Core.Shared;
using Infrastructure.Persistence;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using static Core.Shared.Cryptography.CharGenerator;

namespace Infratructure.Test {
    [TestFixture]
    class PersistenceElasticSearchTest {
        [TestCase]
        public void testSingleInsert() {
            Core.Application.DTOs.Configurations.ElasticSearch _config = new Core.Application.DTOs.Configurations.ElasticSearch();
            _config.nodes = new string[] { "http://localhost:9200/" };
            ElasticSearch s = new ElasticSearch(_config);
            var TestUser = new TestUser() { id = 1, fullname = "Akin Elias", gender = "Gender", phone = "09099999999" };
            bool inserted = s.insert<TestUser>(TestUser).Result;
            Assert.AreEqual(inserted, true);
        }
        [TestCase]
        public void testMultipleInsert() {
            Core.Application.DTOs.Configurations.ElasticSearch _config = new Core.Application.DTOs.Configurations.ElasticSearch();
            _config.nodes = new string[] { "http://localhost:9200/" };
            ElasticSearch s = new ElasticSearch(_config);
            var testUser = new List<TestUser>() { new TestUser { fullname = "Sirat M", gender = "Gender", phone = "09099999999" }, new TestUser { fullname = "Akin Elias II", gender = "Gender", phone = "09099999999" } };
            bool inserted = s.insert<TestUser>(testUser).Result;
            Assert.AreEqual(inserted, true);
        }
        public class TestUser {
            public int id { get; set; } = (int)double.Parse(Cryptography.CharGenerator.genID(5, characterSet.NUMERIC));
            public string fullname { get; set; }
            public string gender { get; set; }
            public string phone { get; set; }
        }
    }
}
