using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Relatorios;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Enrollment reports. Aggregation is performed in SQL (JOIN / GROUP BY), not in memory.
    /// </summary>
    [RoutePrefix("api/relatorios")]
    public class RelatoriosController : ApiController
    {
        private readonly RelatorioService _relatorioService;

        public RelatoriosController(RelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        /// <summary>
        /// Returns every turma with enrolled count and remaining seats from a single SQL LEFT JOIN + GROUP BY.
        /// Turmas with zero enrollments are included.
        /// </summary>
        [HttpGet]
        [Route("alunos-por-turma")]
        public async Task<IHttpActionResult> AlunosPorTurma()
        {
            var relatorio = await _relatorioService.ListarAlunosPorTurmaAsync().ConfigureAwait(false);
            return Ok(relatorio);
        }
    }
}
