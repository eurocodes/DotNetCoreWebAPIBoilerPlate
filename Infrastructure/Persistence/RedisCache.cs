using AutoMapper;
using Infrastructure.Interfaces.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence {
    [RegisterAsScoped]
    public class RedisCache : ICacheService {
        private readonly IMemoryCache memoryCache;
        private readonly IDistributedCache distributedCache;
        private readonly IMapper _mapper;
        public RedisCache(IMemoryCache memoryCache, IDistributedCache distributedCache, IMapper _mapper) {
            this.memoryCache = memoryCache;
            this.distributedCache = distributedCache;
            this._mapper = _mapper;
        }
        public async Task<bool> addWithKey(string key, string value, int expiry = 600) {
            try {
                int slidingExpiry = expiry;
                var data = Encoding.UTF8.GetBytes(value);
                var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(expiry))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(expiry));
                await distributedCache.SetAsync(key, data, options);
                return true;
            } catch {
                return false;
            }
        }

        public async Task<bool> deleteWithKey(string key) {
            try {
                await distributedCache.RemoveAsync(key);
                return true;
            } catch {
                return false;
            }
        }

        public async Task<T> getWithKey<T>(string key) {
            try {
                var data = await distributedCache.GetAsync(key);
                if (data != null) {
                    var dataAsStr = Encoding.UTF8.GetString(data);                    
                    return _mapper.Map<T>(data);
                }
                return default(T);
            } catch {
                return default(T);
            }
        }
    }
}
