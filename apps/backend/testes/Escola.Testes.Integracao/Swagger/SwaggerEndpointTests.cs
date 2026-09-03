using System.Net;
using System.Threading.Tasks;
using Escola.Testes.Integracao.Infra;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Escola.Testes.Integracao.Swagger
{
    [Collection("EnrollmentApi")]
    public class SwaggerEndpointTests
    {
        public SwaggerEndpointTests(EnrollmentDatabaseFixture fixture)
        {
        }

        [Fact]
        public async Task SwaggerUi_ReturnsOk()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var ui = await server.HttpClient.GetAsync("/swagger/ui/index");
                Assert.Equal(HttpStatusCode.OK, ui.StatusCode);

                var swagger = await server.HttpClient.GetAsync("/swagger");
                Assert.True(
                    swagger.StatusCode == HttpStatusCode.OK || swagger.StatusCode == HttpStatusCode.MovedPermanently);

                var root = await server.HttpClient.GetAsync("/");
                Assert.True(
                    root.StatusCode == HttpStatusCode.OK || root.StatusCode == HttpStatusCode.MovedPermanently);
            }
        }

        [Fact]
        public async Task SwaggerDocument_ListsAssignmentRoutesAndHealth()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.HttpClient.GetAsync("/swagger/docs/v1");
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var json = JObject.Parse(await response.Content.ReadAsStringAsync());
                var paths = (JObject)json["paths"];
                Assert.NotNull(paths);
                Assert.True(paths.ContainsKey("/api/alunos"));
                Assert.True(paths.ContainsKey("/api/alunos/{id}"));
                Assert.True(paths.ContainsKey("/api/turmas"));
                Assert.True(paths.ContainsKey("/api/matriculas"));
                Assert.True(paths.ContainsKey("/api/relatorios/alunos-por-turma"));
                Assert.True(paths.ContainsKey("/api/health"));

                var definitions = (JObject)json["definitions"];
                Assert.NotNull(definitions);
                Assert.True(definitions.ContainsKey("AlunoDto"));
                Assert.True(definitions.ContainsKey("AlunoEscritaDto"));
                Assert.True(definitions.ContainsKey("ListaAlunosDto"));
                Assert.True(definitions.ContainsKey("TurmaDto"));
                Assert.True(definitions.ContainsKey("CriarMatriculaDto"));
                Assert.True(definitions.ContainsKey("MatriculaDto"));
                Assert.True(definitions.ContainsKey("RelatorioAlunosPorTurmaDto"));
                Assert.True(definitions.ContainsKey("HealthReport"));
                Assert.True(definitions.ContainsKey("ErrorResponse"));

                Assert.False(string.IsNullOrWhiteSpace((string)definitions["AlunoDto"]["description"]));
                Assert.False(string.IsNullOrWhiteSpace((string)definitions["AlunoDto"]["properties"]["nome"]["description"]));
                Assert.False(string.IsNullOrWhiteSpace((string)definitions["AlunoEscritaDto"]["properties"]["dataNascimento"]["description"]));
                Assert.False(string.IsNullOrWhiteSpace((string)definitions["MatriculaDto"]["properties"]["alunoId"]["description"]));

                Assert.Equal("string", (string)definitions["AlunoDto"]["properties"]["dataNascimento"]["type"]);
                Assert.Equal("date", (string)definitions["AlunoDto"]["properties"]["dataNascimento"]["format"]);
                Assert.Equal("date-time", (string)definitions["AlunoDto"]["properties"]["dataCadastro"]["format"]);
                Assert.Equal("date", (string)definitions["AlunoEscritaDto"]["properties"]["dataNascimento"]["format"]);

                var postAluno = paths["/api/alunos"]["post"];
                Assert.NotNull(postAluno["parameters"]);
                Assert.False(string.IsNullOrWhiteSpace((string)postAluno["description"]));
                AssertResponseRef(postAluno, "201", "AlunoDto");
                AssertResponseRef(postAluno, "400", "ErrorResponse");
                Assert.Null(postAluno["responses"]["200"]);

                var postMatricula = paths["/api/matriculas"]["post"];
                Assert.NotNull(postMatricula["parameters"]);
                Assert.False(string.IsNullOrWhiteSpace((string)postMatricula["description"]));
                AssertResponseRef(postMatricula, "201", "MatriculaDto");
                AssertResponseRef(postMatricula, "400", "ErrorResponse");
                AssertResponseRef(postMatricula, "404", "ErrorResponse");
                AssertResponseRef(postMatricula, "409", "ErrorResponse");

                AssertDocumentedOperation(paths["/api/alunos"]["get"]);
                AssertResponseRef(paths["/api/alunos"]["get"], "200", "ListaAlunosDto");
                AssertResponseRef(paths["/api/alunos"]["get"], "400", "ErrorResponse");

                AssertDocumentedOperation(paths["/api/alunos/{id}"]["get"]);
                AssertResponseRef(paths["/api/alunos/{id}"]["get"], "200", "AlunoDto");
                AssertResponseRef(paths["/api/alunos/{id}"]["get"], "404", "ErrorResponse");

                AssertDocumentedOperation(paths["/api/alunos/{id}"]["put"]);
                AssertResponseRef(paths["/api/alunos/{id}"]["put"], "200", "AlunoDto");
                AssertResponseRef(paths["/api/alunos/{id}"]["put"], "400", "ErrorResponse");
                AssertResponseRef(paths["/api/alunos/{id}"]["put"], "404", "ErrorResponse");

                AssertDocumentedOperation(paths["/api/alunos/{id}"]["delete"]);
                Assert.NotNull(paths["/api/alunos/{id}"]["delete"]["responses"]["204"]);
                Assert.Null(paths["/api/alunos/{id}"]["delete"]["responses"]["204"]["schema"]);
                AssertResponseRef(paths["/api/alunos/{id}"]["delete"], "404", "ErrorResponse");

                AssertDocumentedOperation(paths["/api/turmas"]["get"]);
                AssertDocumentedOperation(paths["/api/relatorios/alunos-por-turma"]["get"]);

                var health = paths["/api/health"]["get"];
                AssertDocumentedOperation(health);
                AssertResponseRef(health, "200", "HealthReport");
                AssertResponseRef(health, "503", "HealthReport");
            }
        }

        private static void AssertDocumentedOperation(JToken operation)
        {
            Assert.NotNull(operation);
            Assert.False(string.IsNullOrWhiteSpace((string)operation["description"]));
        }

        private static void AssertResponseRef(JToken operation, string statusCode, string definitionName)
        {
            var schema = operation["responses"]?[statusCode]?["schema"];
            Assert.NotNull(schema);
            Assert.Equal("#/definitions/" + definitionName, (string)schema["$ref"]);
        }
    }
}
