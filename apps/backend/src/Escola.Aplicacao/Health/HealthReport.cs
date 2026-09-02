using System.Collections.Generic;

namespace Escola.Aplicacao.Health
{
    public class HealthReport
    {
        public const string Healthy = "healthy";
        public const string Unavailable = "unavailable";

        public string Status { get; set; }

        public IDictionary<string, string> Dependencies { get; set; }

        public bool IsHealthy()
        {
            return Status == Healthy;
        }
    }
}
