using Core.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Core.Shared.Cryptography;

namespace Core.Test {
    [TestFixture]
    public class CryptographyTest {
        [TestCase]
        public void hashMethodShouldReturnHexString() {
            string raw = "Some string here";
            string hash = Cryptography.Hash.getHash(raw);
            string hash2 = Cryptography.Hash.getHash(raw+"Some Additional String");
            string hash3 = Cryptography.Hash.getHash(raw + "Some Additional String");
            Assert.True(!string.IsNullOrEmpty(hash), "Hash string should not be empty");
            Assert.AreEqual(hash2, hash3, "Hash string should be reproducible");
            Assert.AreNotEqual(hash, hash2, "Hash string should not collide");
            Assert.True(Utilities.isHexStr(hash), "Hash is a valid hex string");
        }

        [TestCase]
        public void encryptionClassStability() {
            string raw = "Some string here";
            string key = "1234567890986565";            
            string key2 = "678785865765";
            var aes = new Cryptography.AES();
            string digest = aes.Encrypt(raw, key);
            string wrongDecryption = aes.Decrypt(digest, key2);
            string rightDecryption = aes.Decrypt(digest, key);
            Assert.IsTrue(!string.IsNullOrEmpty(digest), "Digest should not be null");
            Assert.IsTrue(rightDecryption == raw, "Decryption should reveal right data");
            Assert.IsTrue(string.IsNullOrEmpty(wrongDecryption), "Decryption should fail on wrong key");
        }

        [TestCase]
        public void charGeneratorClass() {
            string numString = CharGenerator.genID(6, CharGenerator.characterSet.NUMERIC);
            string manyCaseStr = CharGenerator.genID(6, CharGenerator.characterSet.ALPHA_NUMERIC_CASE);
            string manyNoCaseStr = CharGenerator.genID(6, CharGenerator.characterSet.ALPHA_NUMERIC_NON_CASE);
            string guidStr = CharGenerator.genID();
            string hexStr2 = CharGenerator.genID(6, CharGenerator.characterSet.HEX_STRING);
            string lowerAstr = CharGenerator.genID(6, CharGenerator.characterSet.LOWER_ALPHABETS_ONLY);
            string upperAstr = CharGenerator.genID(6, CharGenerator.characterSet.UPPER_ALPHABETS_ONLY);
            Assert.IsTrue(numString.Length == 6, "Length return must be 6");
            Assert.IsTrue(new Regex("^[0-9]+$").IsMatch(numString), "Numeric value expected");

            Assert.IsTrue(manyCaseStr.Length == 6, "Length return must be 6");
            Assert.IsTrue(new Regex("^[0-9A-Za-z]+$").IsMatch(manyCaseStr), "Alpha numeric case with lower and uppercase value expected");

            Assert.IsTrue(manyNoCaseStr.Length == 6, "Length return must be 6");
            Assert.IsTrue(new Regex("^[0-9A-Z]+$").IsMatch(manyNoCaseStr), "Alpha numeric case with uppercase value expected");

            Assert.IsTrue(!string.IsNullOrEmpty(guidStr));

            Assert.IsTrue(hexStr2.Length == 6, "Length return must be 6");
            Assert.IsTrue(new Regex("^[0-9a-z]+$").IsMatch(hexStr2), "Hexadecimal Chars");

            Assert.IsTrue(lowerAstr.Length == 6, "Length return must be 6");
            Assert.IsTrue(new Regex("^[a-z]+$").IsMatch(lowerAstr), "Lower case Chars only");

            Assert.IsTrue(upperAstr.Length == 6, "Length return must be 6");
            Assert.IsTrue(new Regex("^[A-Z]+$").IsMatch(upperAstr), "Upper case Chars only");
        }
    }
}
