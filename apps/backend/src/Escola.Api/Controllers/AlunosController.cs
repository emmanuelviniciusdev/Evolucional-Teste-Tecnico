using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Alunos;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Paginated aluno CRUD. DELETE is a logical deactivation (Ativo = false); the row remains readable.
    /// </summary>
    [RoutePrefix("api/alunos")]
    public class AlunosController : ApiController
    {
        private readonly AlunoService _alunoService;

        public AlunosController(AlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        /// <summary>
        /// Lists alunos with optional case-insensitive name filter.
        /// Query parameters: nome, pagina (default 1), tamanhoPagina (default 10, max 100).
        /// The payload includes the page, the matching total, and the pagination values used.
        /// Inactive alunos are included. Invalid pagination returns HTTP 400.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Listar(string nome = null, int? pagina = null, int? tamanhoPagina = null)
        {
            var lista = await _alunoService.ListarAsync(nome, pagina, tamanhoPagina).ConfigureAwait(false);
            return Ok(lista);
        }

        /// <summary>
        /// Returns an aluno by id, including inactive ones. HTTP 404 when the id does not exist.
        /// </summary>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> ObterPorId(int id)
        {
            var aluno = await _alunoService.ObterPorIdAsync(id).ConfigureAwait(false);
            return Ok(aluno);
        }

        /// <summary>
        /// Creates an active aluno. Email must be a complete address; dataNascimento must be YYYY-MM-DD.
        /// Returns HTTP 201 with Location /api/alunos/{id}.
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Criar([FromBody] AlunoEscritaDto dto)
        {
            var criado = await _alunoService.CriarAsync(dto).ConfigureAwait(false);
            var location = new Uri("/api/alunos/" + criado.Id, UriKind.Relative);
            return Created(location, criado);
        }

        /// <summary>
        /// Updates nome, email, and dataNascimento. Does not change Ativo. HTTP 404 when missing.
        /// </summary>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Atualizar(int id, [FromBody] AlunoEscritaDto dto)
        {
            var atualizado = await _alunoService.AtualizarAsync(id, dto).ConfigureAwait(false);
            return Ok(atualizado);
        }

        /// <summary>
        /// Logical delete: sets Ativo to false and returns HTTP 204. Idempotent when already inactive.
        /// HTTP 404 when the id does not exist. GET still returns the aluno with ativo false.
        /// </summary>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Excluir(int id)
        {
            await _alunoService.ExcluirLogicamenteAsync(id).ConfigureAwait(false);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
