using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Escola.Testes.Integracao.Infra;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using Xunit;

namespace Escola.Testes.Integracao.Matriculas
{
    [Collection("EnrollmentApi")]
    public class MatriculaEndpointTests
    {
        public MatriculaEndpointTests(EnrollmentDatabaseFixture fixture)
        {
            fixture.Reset();
        }

        [Fact]
        public async Task PostMatricula_ValidPair_ReturnsCreatedAndDecrementsSeats()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var aluno = await server.PostJsonAsync("/api/alunos", new
                {
                    nome = "Novo Aluno",
                    email = "novo.aluno@email.com",
                    dataNascimento = "2006-03-14"
                });
                var alunoId = (int)(await aluno.ReadJsonAsync())["id"];

                var before = await RemainingSeatsAsync(server, "3B - Ensino Medio");
                var response = await server.PostJsonAsync("/api/matriculas", new { alunoId, turmaId = 2 });
                var json = Assert.IsType<JObject>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Contains("/api/matriculas/" + (int)json["id"], response.Headers.Location.ToString());

                var after = await RemainingSeatsAsync(server, "3B - Ensino Medio");
                Assert.Equal(before - 1, after);
            }
        }

        [Fact]
        public async Task PostMatricula_MissingIds_ReturnsBadRequest()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/matriculas", new { alunoId = 0, turmaId = 1 });
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostMatricula_UnknownAluno_ReturnsNotFound()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/matriculas", new { alunoId = 99999, turmaId = 1 });
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostMatricula_UnknownTurma_ReturnsNotFound()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/matriculas", new { alunoId = 1, turmaId = 99999 });
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostMatricula_InactiveAluno_ReturnsConflict()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/matriculas", new { alunoId = 4, turmaId = 2 });
                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostMatricula_FullTurma_ReturnsConflict()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/matriculas", new { alunoId = 1, turmaId = 4 });
                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostMatricula_Duplicate_ReturnsConflict()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/matriculas", new { alunoId = 1, turmaId = 1 });
                Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostMatricula_InvalidatesTurmaCacheOnRedisDb1()
        {
            using (var mux = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            using (var server = TestServerExtensions.CreateApi())
            {
                var db0 = mux.GetDatabase(0);
                var db1 = mux.GetDatabase(1);
                var beforeDb0 = db0.StringGet("turmas:listagem");

                await server.GetJsonAsync("/api/turmas");
                Assert.True(db1.KeyExists("turmas:listagem"));

                var aluno = await server.PostJsonAsync("/api/alunos", new
                {
                    nome = "Cache Aluno",
                    email = "cache.aluno@email.com",
                    dataNascimento = "2006-03-14"
                });
                var alunoId = (int)(await aluno.ReadJsonAsync())["id"];
                var enroll = await server.PostJsonAsync("/api/matriculas", new { alunoId, turmaId = 2 });
                Assert.Equal(HttpStatusCode.Created, enroll.StatusCode);
                Assert.False(db1.KeyExists("turmas:listagem"));

                var afterList = await server.GetJsonAsync("/api/turmas");
                var turmas = Assert.IsType<JArray>(await afterList.ReadJsonAsync());
                var tresB = turmas.Single(t => (string)t["nome"] == "3B - Ensino Medio");
                Assert.Equal(29, (int)tresB["vagasDisponiveis"]);
                Assert.Equal(beforeDb0, db0.StringGet("turmas:listagem"));
            }
        }

        private static async Task<int> RemainingSeatsAsync(Microsoft.Owin.Testing.TestServer server, string nome)
        {
            var response = await server.GetJsonAsync("/api/turmas");
            var json = Assert.IsType<JArray>(await response.ReadJsonAsync());
            return (int)json.Single(t => (string)t["nome"] == nome)["vagasDisponiveis"];
        }
    }
}
