using System;
using System.Threading.Tasks;
using Escola.Dominio;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Escola.Infraestrutura.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _redis.GetDatabase().StringGetAsync(key).ConfigureAwait(false);
            if (value.IsNullOrEmpty)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonConvert.SerializeObject(value);
            await _redis.GetDatabase().StringSetAsync(key, json, expiration).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string key)
        {
            await _redis.GetDatabase().KeyDeleteAsync(key).ConfigureAwait(false);
        }
    }
}
