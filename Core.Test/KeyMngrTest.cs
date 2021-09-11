using Core.Application.DTOs.Configurations;
using Core.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Test {
    [TestFixture]
    public class KeyMngrTest {
        [TestCase]
        public void testKeyManager() {
            List<KeysTemplate> keys = new List<KeysTemplate>() { new KeysTemplate { salt = "896ghvjbsdf87sdjnbsdsd", saltIndex = 7 }, new KeysTemplate { salt = "896ghvjbsdf87sdjnbsdsd", saltIndex = 3 } };
            KeyMngr kmn = new KeyMngr(keys);
            string sensitiveString = "SuperSensitive";
            string saltedHash = kmn.getDigest(sensitiveString, 0);
            string saltedHash2 = kmn.getDigest(sensitiveString, 1);
            string unsaltedHash = kmn.getDigest(sensitiveString, "");

            Assert.IsTrue(!string.IsNullOrWhiteSpace(saltedHash), "The hash is not empty");
            Assert.IsTrue(saltedHash != saltedHash2, "Salted hash should have different value");
            Assert.IsTrue(saltedHash != unsaltedHash, "Salted hash should have different value");
            Assert.IsTrue(saltedHash != sensitiveString, "Original string should never be revealed");
        }
    }
}
