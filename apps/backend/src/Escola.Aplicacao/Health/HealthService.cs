using System.Collections.Generic;
using System.Threading.Tasks;
using Escola.Dominio;

namespace Escola.Aplicacao.Health
{
    public class HealthService
    {
        private readonly IDependencyChecker _dependencyChecker;

        public HealthService(IDependencyChecker dependencyChecker)
        {
            _dependencyChecker = dependencyChecker;
        }

        public async Task<HealthReport> CheckAsync()
        {
            var sqlServerHealthy = await _dependencyChecker.CanReachSqlServerAsync().ConfigureAwait(false);
            var redisHealthy = await _dependencyChecker.CanReachRedisAsync().ConfigureAwait(false);

            var sqlServerStatus = sqlServerHealthy ? HealthReport.Healthy : HealthReport.Unavailable;
            var redisStatus = redisHealthy ? HealthReport.Healthy : HealthReport.Unavailable;
            var overallHealthy = sqlServerHealthy && redisHealthy;

            return new HealthReport
            {
                Status = overallHealthy ? HealthReport.Healthy : HealthReport.Unavailable,
                Dependencies = new Dictionary<string, string>
                {
                    { "sqlServer", sqlServerStatus },
                    { "redis", redisStatus }
                }
            };
        }
    }
}
