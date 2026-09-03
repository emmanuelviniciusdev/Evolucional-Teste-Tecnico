using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Escola.Api.Filters;
using Escola.Dominio.Excecoes;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Escola.Testes.Unitarios.Filters
{
    public class JsonExceptionFilterTests
    {
        [Fact]
        public async Task OnException_ValidationException_ReturnsBadRequestWithPtBrError()
        {
            var response = ExecuteAsync(new ValidationException("Nome é obrigatório."));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            await AssertErrorAsync(response, "Nome é obrigatório.");
        }

        [Fact]
        public async Task OnException_NotFoundException_ReturnsNotFoundWithPtBrError()
        {
            var response = ExecuteAsync(new NotFoundException("Aluno não encontrado."));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            await AssertErrorAsync(response, "Aluno não encontrado.");
        }

        [Fact]
        public async Task OnException_ConflictException_ReturnsConflictWithPtBrError()
        {
            var response = ExecuteAsync(new ConflictException("Turma sem vagas disponíveis."));

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            await AssertErrorAsync(response, "Turma sem vagas disponíveis.");
        }

        [Fact]
        public async Task OnException_UnexpectedException_ReturnsInternalServerError()
        {
            var response = ExecuteAsync(new System.InvalidOperationException("boom"));

            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            await AssertErrorAsync(response, "An unexpected error occurred.");
        }

        private static HttpResponseMessage ExecuteAsync(System.Exception exception)
        {
            var config = new HttpConfiguration();
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/alunos");
            request.SetConfiguration(config);

            var actionContext = new HttpActionContext
            {
                ControllerContext = new HttpControllerContext
                {
                    Configuration = config,
                    Request = request
                }
            };

            var executed = new HttpActionExecutedContext(actionContext, exception);
            new JsonExceptionFilter().OnException(executed);
            return executed.Response;
        }

        private static async Task AssertErrorAsync(HttpResponseMessage response, string expected)
        {
            var json = JObject.Parse(await response.Content.ReadAsStringAsync());
            json["error"].Value<string>().Should().Be(expected);
        }
    }
}
