using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Escola.Aplicacao.Alunos;
using Swashbuckle.Swagger.Annotations;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Paginated aluno CRUD. DELETE is a logical deactivation (Ativo = false); GET omits inactive alunos.
    /// </summary>
    [RoutePrefix("api/alunos")]
    [SwaggerResponse(HttpStatusCode.InternalServerError, "Unexpected error.", typeof(ErrorResponse))]
    public class AlunosController : ApiController
    {
        private readonly AlunoService _alunoService;

        public AlunosController(AlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        /// <summary>
        /// Lists active alunos with optional case-insensitive name filter.
        /// </summary>
        /// <remarks>
        /// Query parameters: nome, pagina (default 1), tamanhoPagina (default 10, max 100).
        /// The payload includes the page, the matching total of active alunos, and the pagination values used.
        /// Inactive alunos are omitted. Invalid pagination returns HTTP 400.
        /// </remarks>
        /// <param name="nome">Optional case-insensitive name filter.</param>
        /// <param name="pagina">1-based page number. Defaults to 1.</param>
        /// <param name="tamanhoPagina">Page size. Defaults to 10; maximum 100.</param>
        [HttpGet]
        [Route("")]
        [ResponseType(typeof(ListaAlunosDto))]
        [SwaggerResponse(HttpStatusCode.OK, "Paginated active alunos.", typeof(ListaAlunosDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Invalid pagination.", typeof(ErrorResponse))]
        public async Task<IHttpActionResult> Listar(string nome = null, int? pagina = null, int? tamanhoPagina = null)
        {
            var lista = await _alunoService.ListarAsync(nome, pagina, tamanhoPagina).ConfigureAwait(false);
            return Ok(lista);
        }

        /// <summary>
        /// Returns an active aluno by id.
        /// </summary>
        /// <remarks>HTTP 404 when the id does not exist or the aluno is inactive.</remarks>
        [HttpGet]
        [Route("{id:int}")]
        [ResponseType(typeof(AlunoDto))]
        [SwaggerResponse(HttpStatusCode.OK, "Active aluno.", typeof(AlunoDto))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Aluno does not exist or is inactive.", typeof(ErrorResponse))]
        public async Task<IHttpActionResult> ObterPorId(int id)
        {
            var aluno = await _alunoService.ObterPorIdAsync(id).ConfigureAwait(false);
            return Ok(aluno);
        }

        /// <summary>
        /// Creates an active aluno.
        /// </summary>
        /// <remarks>
        /// Email must be a complete address; dataNascimento must be YYYY-MM-DD.
        /// Returns HTTP 201 with Location /api/alunos/{id}.
        /// </remarks>
        [HttpPost]
        [Route("")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.Created, "Created aluno.", typeof(AlunoDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Validation failed.", typeof(ErrorResponse))]
        public async Task<IHttpActionResult> Criar([FromBody] AlunoEscritaDto dto)
        {
            var criado = await _alunoService.CriarAsync(dto).ConfigureAwait(false);
            var location = new Uri("/api/alunos/" + criado.Id, UriKind.Relative);
            return Created(location, criado);
        }

        /// <summary>
        /// Updates nome, email, and dataNascimento.
        /// </summary>
        /// <remarks>Does not change Ativo. HTTP 404 when the aluno is missing.</remarks>
        [HttpPut]
        [Route("{id:int}")]
        [ResponseType(typeof(AlunoDto))]
        [SwaggerResponse(HttpStatusCode.OK, "Updated aluno.", typeof(AlunoDto))]
        [SwaggerResponse(HttpStatusCode.BadRequest, "Validation failed.", typeof(ErrorResponse))]
        [SwaggerResponse(HttpStatusCode.NotFound, "Aluno does not exist.", typeof(ErrorResponse))]
        public async Task<IHttpActionResult> Atualizar(int id, [FromBody] AlunoEscritaDto dto)
        {
            var atualizado = await _alunoService.AtualizarAsync(id, dto).ConfigureAwait(false);
            return Ok(atualizado);
        }

        /// <summary>
        /// Logical delete: sets Ativo to false and returns HTTP 204.
        /// </summary>
        /// <remarks>
        /// Idempotent when already inactive. HTTP 404 when the id does not exist.
        /// Later GET list and GET by id omit the aluno.
        /// </remarks>
        [HttpDelete]
        [Route("{id:int}")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.NoContent, "Aluno deactivated.")]
        [SwaggerResponse(HttpStatusCode.NotFound, "Aluno id does not exist.", typeof(ErrorResponse))]
        public async Task<IHttpActionResult> Excluir(int id)
        {
            await _alunoService.ExcluirLogicamenteAsync(id).ConfigureAwait(false);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
