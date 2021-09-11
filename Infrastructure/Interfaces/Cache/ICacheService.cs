using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Cache {
    public interface ICacheService {
        Task<T> getWithKey<T>(string key);
        Task<bool> addWithKey(string key, string value, int expiry = 600);
        Task<bool> deleteWithKey(string key);
    }
}
