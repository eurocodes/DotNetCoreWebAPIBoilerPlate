using Core.Shared;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Test {
    [TestFixture]
    public class UtilitiesTest {
        private string dictionary;
        public UtilitiesTest() {
            dictionary = "9RaqM5sBtxrv1T4b7HWQKCn80c6D2ZwfSNXJudUomLAeygiYFVIlPhpzEjO3kG";
        }
        [TestCase]
        public void testAllPatternUtilitiesFeatures() {
            bool isAlphanumeric = Utilities.isAlphaNumeric("9809JARepoM");
            bool notAlphanumeric = Utilities.isAlphaNumeric("90000000");
            bool isAnMd5 = Utilities.isMD5("5ac749fbeec93607fc28d666be85e73a");
            bool isNotMd5 = Utilities.isMD5("5ac749fbeec93607666be85e73a");
            bool isHexStr = Utilities.isHexStr("5ac749fbeec93607fc28d666be85e73a");
            bool isNotHexStr = Utilities.isHexStr("5ac749fbeec93607fc28dy666be85e73a");
            bool isPasswordCase = Utilities.passwordCase("P@ssw0rd");
            bool isNotPasswordCase = Utilities.passwordCase("Passw0rd");
            bool isPhone = Utilities.isPhone("08167565432");
            bool isPhone2 = Utilities.isPhone("2348167565432");
            bool isNotPhone = Utilities.isPhone("55555555555");
            bool isNotPhone2 = Utilities.isPhone("04187654329");
            Assert.IsTrue(isAlphanumeric, "Should match a valid alphanumeric, regardless of case");
            Assert.IsFalse(notAlphanumeric, "Should not match a non AN");
            Assert.IsTrue(isAnMd5, "Should match a valid MD5");
            Assert.IsFalse(isNotMd5, "Should not match an invalid/curruped MD5");
            Assert.IsTrue(isHexStr, "Should match a valid Hex string");
            Assert.IsFalse(isNotHexStr, "Should not match an invalid/currupted Hex str");
            Assert.IsTrue(isPasswordCase, "Should match a valid password. At least 1 each of Upper case, special chars, number");
            Assert.IsFalse(isNotPasswordCase, "Should not match a password which does not satisfy any of the above");
            Assert.IsTrue(isPhone, "Should match a valid local phone without extension");
            Assert.IsTrue(isPhone2, "Should match a valid local phone with extension");
            Assert.IsFalse(isNotPhone, "Should not match an invalid extension");
            Assert.IsFalse(isNotPhone2, "Should not match an invalid phone without extension");
        }

        [TestCase]
        public void testReproducableIDCryptography() {
            string dictionary = "qAZcdO01n68fxeo5wFsE7zUKYBlXbIS9rNjhGWyCVMiDpJv43mLTRaPHu2Qkgt";
            long day = 1609462870;
            int daysPassed = Cryptography.CharGenerator.daysPassedSinceYearBegin(day);
            int daysPassedII = Cryptography.CharGenerator.daysPassedSinceYearBegin(1611968470);
            Assert.IsTrue(daysPassed == 0, "Days passed should be 0");
            Assert.IsTrue(daysPassedII == 29, "Days passed should be 29");
        }

        [TestCase]
        public void testDateUtilitiesFeatures() {
            long getTimeStamp = Utilities.getTimeStamp(1970, 1, 1, 0, 0, 0, 0);
            (DateTime modernDate, double unixTimestamp, double unixTimestampMS) = Utilities.getTodayDate();
            int ms = (int)(unixTimestampMS - unixTimestamp);
            DateTime date = Utilities.unixTimeStampToDateTime(unixTimestamp);
            Assert.IsTrue(ms < 1000, "Difference between UnixSeconds and UnixMillisecs should be less than 1000");
            Assert.AreEqual(date, modernDate, "Reversal of Unix to dateTime should be consistent");
            Assert.IsTrue(getTimeStamp == 0, "Unix at said day should be 0 seconds old");
        }

        [TestCase]
        public void testJObjectHelper() {
            List<object> children = new List<object> { new { roleName = "Supervisor", status = 0 }, new { roleName = "Staff", status = 1 } };
            object child = new { job = "Engeneer", salary = 200000 };
            object randomObj = new { name = "Some Name", age = 87, isAdmin = false, Roles = children, JD = child };
            JObject jObjectRandomObj = Utilities.asJObject(randomObj);
            string name = Utilities.findString(jObjectRandomObj, "name");
            string notFoundAttr = Utilities.findString(jObjectRandomObj, "name0");
            double? age = Utilities.findNumber(jObjectRandomObj, "age");
            JArray roles = Utilities.findArray(jObjectRandomObj, "Roles");
            JObject jd = Utilities.findObj(jObjectRandomObj, "JD");
            Assert.IsTrue(jObjectRandomObj != null, "Jobject conversion Succeded");
            Assert.IsTrue(name == "Some Name", "findstring should return a valid value");
            Assert.IsTrue(notFoundAttr == null, "findstring should return null for invalid idex/key");
            Assert.IsTrue(age == 87, "Find number should return number as in original object indexed");
            Assert.IsTrue(roles != null, "findArray should return collection as JArray");
            Assert.IsTrue(roles.Count == 2, "findArray should return exactly same count as in original object");
            Assert.IsTrue(jd != null, "findObj should return object as JObject");
            Assert.IsTrue(jd["job"].ToString() == "Engeneer", "findObj should return correctly index value");
        }


        /* The following Tests the aDayCode() which generate a digest for a particular identity valid only for a day. */
        [TestCase]
        public void aDayCodeMustNotBeNull() {
            //Generate a code
            string code = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary);
            Assert.AreNotEqual(code, null, "code generated cannot be null");
        }
        [TestCase]
        public void methodMustReturnConsistentValue() {            
            //Generate a code
            string codeA = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary);
            string codeB = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary);
            Assert.AreEqual(codeA, codeB, "Code generated must be reproducible");
        }
        [TestCase]
        public void codeGenerationWithoutDateDefaultstoToday() {            
            //Generate code
            string codeA = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary);
            string codeB = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary, Utilities.getTodayDate().unixTimestamp);
            Assert.AreEqual(codeA, codeB, "The code method takes optional parameter that defaults to today");
        }
        [TestCase]
        public void codeMustdiffer() {            
            //Generate a code
            string codeA = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary);
            string codeB = Cryptography.CharGenerator.aDayCode("TestID12", this.dictionary);
            Assert.AreNotEqual(codeA, codeB, "Code must be unique for each identifier");
        }
        [TestCase]
        public void codediffdMustdiffer() {           
            //Generate a code
            string busCode = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary);
            string busCode2 = Cryptography.CharGenerator.aDayCode("TestID", this.dictionary, Utilities.getTodayDate().unixTimestamp - 86400);
            Assert.AreNotEqual(busCode, busCode2, "Code generated on different days must not match");
        }
    }
}
