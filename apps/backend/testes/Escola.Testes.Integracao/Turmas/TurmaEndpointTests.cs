using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Escola.Testes.Integracao.Infra;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using Xunit;

namespace Escola.Testes.Integracao.Turmas
{
    [Collection("EnrollmentApi")]
    public class TurmaEndpointTests
    {
        public TurmaEndpointTests(EnrollmentDatabaseFixture fixture)
        {
            fixture.Reset();
        }

        [Fact]
        public async Task GetTurmas_ReturnsRemainingSeatsIncludingFullTurma()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.GetJsonAsync("/api/turmas");
                var json = Assert.IsType<JArray>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(4, json.Count);

                var lotada = json.Single(t => (string)t["nome"] == "Turma Lotada");
                Assert.Equal(0, (int)lotada["vagasDisponiveis"]);

                var intensiva = json.Single(t => (string)t["nome"] == "Turma Intensiva");
                Assert.Equal(1, (int)intensiva["vagasDisponiveis"]);
            }
        }

        [Fact]
        public async Task GetTurmas_WritesCacheToRedisDatabase1Only()
        {
            using (var mux = ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false"))
            {
                var db0 = mux.GetDatabase(0);
                var before = db0.StringGet("turmas:listagem");

                using (var server = TestServerExtensions.CreateApi())
                {
                    var response = await server.GetJsonAsync("/api/turmas");
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }

                Assert.Equal(before, db0.StringGet("turmas:listagem"));
                Assert.True(mux.GetDatabase(1).KeyExists("turmas:listagem"));
            }
        }
    }
}
