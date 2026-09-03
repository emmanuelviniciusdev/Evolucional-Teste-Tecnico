using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Health;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Readiness probe for SQL Server and Redis.
    /// </summary>
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        private readonly HealthService _healthService;

        public HealthController(HealthService healthService)
        {
            _healthService = healthService;
        }

        /// <summary>
        /// Returns HTTP 200 when SQL Server and Redis are reachable, otherwise HTTP 503.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Get()
        {
            var report = await _healthService.CheckAsync().ConfigureAwait(false);
            if (report.IsHealthy())
            {
                return Ok(report);
            }

            return Content(HttpStatusCode.ServiceUnavailable, report);
        }
    }
}
