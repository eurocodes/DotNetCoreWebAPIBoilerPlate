using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Application.Interfaces.Auth {
    public interface IIdentityMngr {
        string message { get; }
        bool valid { get; }
        bool sessionValid();
        string getJWTIdentity(Dictionary<string, string> identity, int expiry = 0);
        T getProfile<T>();
    }
}
