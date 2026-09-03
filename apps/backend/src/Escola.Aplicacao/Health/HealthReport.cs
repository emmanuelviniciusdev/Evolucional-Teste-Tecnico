using System.Collections.Generic;

namespace Escola.Aplicacao.Health
{
    /// <summary>
    /// Readiness result for SQL Server and Redis.
    /// </summary>
    public class HealthReport
    {
        public const string Healthy = "healthy";
        public const string Unavailable = "unavailable";

        /// <summary>Overall status: healthy or unavailable.</summary>
        public string Status { get; set; }

        /// <summary>Per-dependency status keyed by sqlServer and redis.</summary>
        public IDictionary<string, string> Dependencies { get; set; }

        public bool IsHealthy()
        {
            return Status == Healthy;
        }
    }
}
