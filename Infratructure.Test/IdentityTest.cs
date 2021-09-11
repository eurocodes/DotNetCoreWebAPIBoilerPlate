using Infrastructure.Services;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infratructure.Test {
    [TestFixture]
    public class IdentityTest {
        private JWTIdentity id;
        [SetUp]
        public void SetUp() {
            this.id = new JWTIdentity("Filarial2472893734434234545345345346356456456456745643564356345326324525412351235122893787873498");
        }

        [TestCase]
        public void generateAValidBase64CodeOfIdentity() {
            string token = getToken();
            Assert.IsTrue(!string.IsNullOrEmpty(token), "Token Should not be Null or Empty");
            string[] tokenParts = token.Split('.');
            Assert.IsTrue(tokenParts.Length == 3, "A valid Token has three parts");
        }
        [TestCase]
        public void aValidTokenShouldBeVerifiableUsingLifetimeOrSettingLifeTimeToFalse() {
            string token = getToken();
            JObject data = id.verifyToken(token, true);
            Assert.IsTrue(data != null, "A succesfully Decoded token should not return a null");
            profile p = data.ToObject<profile>();
            Assert.AreEqual(p.fullname, "Some Username");
            data = id.verifyToken(token, false);
            Assert.IsTrue(data != null, "false should ignore date");
        }

        public string getToken() {
            var profile = new Dictionary<string, string>();
            profile.Add("username", "MyUsername");
            profile.Add("fullname", "Some Username");
            profile.Add("isAdmin", "");
            profile.Add("age", "67");            
            string token = id.getToken(profile);
            return token;
        }

        public class profile {
            public string username { get; set; }
            public string fullname { get; set; }
            public string isAdmin { get; set; }
            public string age { get; set; }
        }
    }
}
