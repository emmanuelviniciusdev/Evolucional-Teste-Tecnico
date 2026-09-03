using System;
using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Matriculas;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Enrolls an aluno in a turma. Insert and seat decrement run in one database transaction.
    /// </summary>
    [RoutePrefix("api/matriculas")]
    public class MatriculasController : ApiController
    {
        private readonly MatriculaService _matriculaService;

        public MatriculasController(MatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        /// <summary>
        /// Creates a matrícula when the aluno is active, the turma has remaining seats, and the pair is unique.
        /// Both the Matricula insert and VagasDisponiveis decrement commit together.
        /// Returns HTTP 201 with Location /api/matriculas/{id}. HTTP 409 for inactive aluno, no seats, or duplicate.
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Criar([FromBody] CriarMatriculaDto dto)
        {
            var criada = await _matriculaService.CriarAsync(dto).ConfigureAwait(false);
            var location = new Uri("/api/matriculas/" + criada.Id, UriKind.Relative);
            return Created(location, criada);
        }
    }
}
