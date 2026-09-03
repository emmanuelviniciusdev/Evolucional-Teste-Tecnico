using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Escola.Aplicacao.Health;
using Swashbuckle.Swagger.Annotations;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Readiness probe for SQL Server and Redis.
    /// </summary>
    [RoutePrefix("api/health")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "Unexpected error.", typeof(ErrorResponse))]
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
        /// <remarks>
        /// Payload includes overall status and per-dependency status (sqlServer, redis).
        /// </remarks>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(HealthReport))]
        [SwaggerResponse(HttpStatusCode.OK, "SQL Server and Redis are reachable.", typeof(HealthReport))]
        [SwaggerResponse(HttpStatusCode.ServiceUnavailable, "A dependency is down.", typeof(HealthReport))]
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
