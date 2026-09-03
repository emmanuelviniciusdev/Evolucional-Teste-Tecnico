using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Turmas;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Turma listing with remaining seats. Served from Redis cache (key turmas:listagem, TTL 5 minutes)
    /// with SQL fallback when cache is down. Cache is invalidated after a successful matrícula.
    /// </summary>
    [RoutePrefix("api/turmas")]
    public class TurmasController : ApiController
    {
        private readonly TurmaService _turmaService;

        public TurmasController(TurmaService turmaService)
        {
            _turmaService = turmaService;
        }

        /// <summary>
        /// Lists every turma including remaining seats from VagasDisponiveis. Uses Redis cache-aside.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Listar()
        {
            var turmas = await _turmaService.ListarAsync().ConfigureAwait(false);
            return Ok(turmas);
        }
    }
}
