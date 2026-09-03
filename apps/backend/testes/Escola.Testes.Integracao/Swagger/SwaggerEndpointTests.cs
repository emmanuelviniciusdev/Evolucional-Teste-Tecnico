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
                var ui = await server.HttpClient.GetAsync("/swagger");
                Assert.Equal(HttpStatusCode.OK, ui.StatusCode);

                var root = await server.HttpClient.GetAsync("/");
                Assert.Equal(HttpStatusCode.OK, root.StatusCode);
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

                var postAluno = paths["/api/alunos"]["post"];
                Assert.NotNull(postAluno["parameters"]);
                Assert.NotNull(json["definitions"]);
            }
        }
    }
}
