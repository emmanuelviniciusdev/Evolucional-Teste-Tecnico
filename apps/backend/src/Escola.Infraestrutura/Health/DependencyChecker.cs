using System;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio;
using StackExchange.Redis;

namespace Escola.Infraestrutura.Health
{
    public class DependencyChecker : IDependencyChecker
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IConnectionMultiplexer _redis;

        public DependencyChecker(IConnectionFactory connectionFactory, IConnectionMultiplexer redis)
        {
            _connectionFactory = connectionFactory;
            _redis = redis;
        }

        public async Task<bool> CanReachSqlServerAsync()
        {
            try
            {
                using (var connection = _connectionFactory.Create())
                {
                    var result = await connection.QuerySingleAsync<int>("SELECT 1").ConfigureAwait(false);
                    return result == 1;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CanReachRedisAsync()
        {
            try
            {
                if (_redis == null || !_redis.IsConnected)
                {
                    return false;
                }

                await _redis.GetDatabase().PingAsync().ConfigureAwait(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
