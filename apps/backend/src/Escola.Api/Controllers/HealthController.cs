using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Health;

namespace Escola.Api.Controllers
{
    [RoutePrefix("api/health")]
    public class HealthController : ApiController
    {
        private readonly HealthService _healthService;

        public HealthController(HealthService healthService)
        {
            _healthService = healthService;
        }

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
