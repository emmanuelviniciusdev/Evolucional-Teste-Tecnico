using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Escola.Aplicacao.Alunos;

namespace Escola.Api.Controllers
{
    /// <summary>
    /// Paginated aluno CRUD. DELETE is a logical deactivation (Ativo = false); GET omits inactive alunos.
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
        /// Lists active alunos with optional case-insensitive name filter.
        /// Query parameters: nome, pagina (default 1), tamanhoPagina (default 10, max 100).
        /// The payload includes the page, the matching total of active alunos, and the pagination values used.
        /// Inactive alunos are omitted. Invalid pagination returns HTTP 400.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Listar(string nome = null, int? pagina = null, int? tamanhoPagina = null)
        {
            var lista = await _alunoService.ListarAsync(nome, pagina, tamanhoPagina).ConfigureAwait(false);
            return Ok(lista);
        }

        /// <summary>
        /// Returns an active aluno by id. HTTP 404 when the id does not exist or the aluno is inactive.
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
        /// HTTP 404 when the id does not exist. Later GET list and GET by id omit the aluno.
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
