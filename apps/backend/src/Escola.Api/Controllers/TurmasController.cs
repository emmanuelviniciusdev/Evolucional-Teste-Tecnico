using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Escola.Aplicacao.Turmas;
using Swashbuckle.Swagger.Annotations;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Turma listing with remaining seats.
    /// </summary>
    /// <remarks>
    /// Served from Redis cache (key turmas:listagem, TTL 5 minutes) with SQL fallback when cache is down.
    /// Cache is invalidated after a successful matrícula.
    /// </remarks>
    [RoutePrefix("api/turmas")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "Unexpected error.", typeof(ErrorResponse))]
    public class TurmasController : ApiController
    {
        private readonly TurmaService _turmaService;

        public TurmasController(TurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        /// <summary>
        /// Lists every turma including remaining seats from VagasDisponiveis.
        /// </summary>
        /// <remarks>Uses Redis cache-aside (key turmas:listagem, TTL 5 minutes) with SQL fallback.</remarks>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(IList<TurmaDto>))]
        [SwaggerResponse(HttpStatusCode.OK, "All turmas with remaining seats.", typeof(IList<TurmaDto>))]
        public async Task<IHttpActionResult> Listar()
        {
            var turmas = await _turmaService.ListarAsync().ConfigureAwait(false);
            return Ok(turmas);
        }
    }
}
