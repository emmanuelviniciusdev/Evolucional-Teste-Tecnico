using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Matriculas;
using Swashbuckle.Swagger.Annotations;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Enrolls an aluno in a turma.
    /// </summary>
    /// <remarks>Insert and seat decrement run in one database transaction.</remarks>
    [RoutePrefix("api/matriculas")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "Unexpected error.", typeof(ErrorResponse))]
    public class MatriculasController : ApiController
    {
        private readonly MatriculaService _matriculaService;

        public MatriculasController(MatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
        }

        /// <summary>
        /// Creates a matrícula when the aluno is active, the turma has remaining seats, and the pair is unique.
        /// </summary>
        /// <remarks>
        /// Both the Matricula insert and VagasDisponiveis decrement commit together.
        /// Returns HTTP 201 with Location /api/matriculas/{id}. HTTP 409 for inactive aluno, no seats, or duplicate.
        /// </remarks>
        [HttpPost]
        [Route("")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.Created, "Created matrícula.", typeof(MatriculaDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Aluno and turma are required.", typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Aluno or turma does not exist.", typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.Conflict, "Inactive aluno, no remaining seats, or duplicate pair.", typeof(ErrorResponse))]
        public async Task<IHttpActionResult> Criar([FromBody] CriarMatriculaDto dto)
        {
            var criada = await _matriculaService.CriarAsync(dto).ConfigureAwait(false);
            var location = new Uri("/api/matriculas/" + criada.Id, UriKind.Relative);
            return Created(location, criada);
        }
    }
}
