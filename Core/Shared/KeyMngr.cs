using Core.Application.DTOs.Configurations;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Shared {
    public class KeyMngr {
        private readonly List<KeysTemplate> passwordSalt;
        public KeyMngr(IOptionsMonitor<SystemVariables> config) {
            passwordSalt = config.CurrentValue.PasswordSalt;
        }
        public KeyMngr(List<KeysTemplate> config) {
            passwordSalt = config;
        }
        public string getDigest(string raw, int index = 0) {
            KeysTemplate aKey = this.passwordSalt[index];
            string rawData = string.Concat(aKey.salt.Substring(0, aKey.saltIndex), raw, aKey.salt.Substring(aKey.saltIndex, aKey.salt.Length - aKey.saltIndex));
            return Cryptography.Hash.getHash(rawData);
        }
        public string getDigest(string raw, string enckey) {
            if (enckey == null)
                return null;
            string rawData = string.Concat(enckey, raw);
            return Cryptography.Hash.getHash(rawData);
        }
    }
}
