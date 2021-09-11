using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Core.Application.Interfaces.Auth;
using Core.Application.DTOs.Configurations;
using NetCore.AutoRegisterDi;

namespace Infrastructure.Services {
    [RegisterAsScoped]
    public class IdentityMngr : IIdentityMngr {
        private readonly SystemVariables _config;
        private readonly IHttpContextAccessor _contextAccessor;
        private protected IHeaderDictionary headers;
        private protected JObject profile;
        public bool valid { get; private set; }
        public string message { get; private set; }
        private readonly JWTIdentity tokenMngr;
        public IdentityMngr(IOptionsMonitor<SystemVariables> config, IHttpContextAccessor contextAccessor) {
            this._config = config.CurrentValue;
            this._contextAccessor = contextAccessor;
            this.tokenMngr = new JWTIdentity(_config.jwtsecret);
            loadProfile();
        }

        public bool sessionValid() {
            loadProfile(true);
            return valid;
        }

        private void loadProfile(bool enforceExpiryCheck = false) {
            try {
                this.headers = _contextAccessor.HttpContext.Request.Headers;
                string h = headers["Authorization"];
                if (!string.IsNullOrEmpty(h)) {
                    string[] auths = h.Trim().Split(" ");
                    if (auths.Length >= 2) {
                        h = auths[1];
                        profile = tokenMngr.verifyToken(h, enforceExpiryCheck? enforceExpiryCheck : _config.identityExpires);
                        valid = profile != null;
                        if (!valid)
                            message = "The authentication is invalid or expired";
                    } else { valid = false; this.message = "Authorization Header is not valid. Login again or contact admin"; }
                } else { valid = false; this.message = "Authorization Header is missing in request"; }
            } catch {
                valid = false;
                this.message = "Authorization Header is missing in request";
            }
        }

        public string getJWTIdentity(Dictionary<string, string> identity, int expiry = 0) {
            return tokenMngr.getToken(identity, expiry < 1? _config.identityExpiryMins : expiry);
        }

        public T getProfile<T>() {
            return profile.ToObject<T>();
        }
        
    }
}
