using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Escola.Aplicacao.Relatorios;
using Swashbuckle.Swagger.Annotations;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Enrollment reports.
    /// </summary>
    /// <remarks>Aggregation is performed in SQL (JOIN / GROUP BY), not in memory.</remarks>
    [RoutePrefix("api/relatorios")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "Unexpected error.", typeof(ErrorResponse))]
    public class RelatoriosController : ApiController
    {
        private readonly RelatorioService _relatorioService;

        public RelatoriosController(RelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        /// <summary>
        /// Returns every turma with enrolled count and remaining seats.
        /// </summary>
        /// <remarks>
        /// Produced by a single SQL LEFT JOIN + GROUP BY. Turmas with zero enrollments are included.
        /// </remarks>
        [HttpGet]
        [Route("alunos-por-turma")]
        [ResponseType(typeof(IList<RelatorioAlunosPorTurmaDto>))]
        [SwaggerResponse(HttpStatusCode.OK, "Enrollment counts per turma.", typeof(IList<RelatorioAlunosPorTurmaDto>))]
        public async Task<IHttpActionResult> AlunosPorTurma()
        {
            var relatorio = await _relatorioService.ListarAlunosPorTurmaAsync().ConfigureAwait(false);
            return Ok(relatorio);
        }
    }
}
