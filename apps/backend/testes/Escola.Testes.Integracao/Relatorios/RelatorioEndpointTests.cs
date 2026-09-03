using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Dapper;
using Escola.Testes.Integracao.Infra;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Escola.Testes.Integracao.Relatorios
{
    [Collection("EnrollmentApi")]
    public class RelatorioEndpointTests
    {
        public RelatorioEndpointTests(EnrollmentDatabaseFixture fixture)
        {
            fixture.Reset();
        }

        [Fact]
        public async Task GetAlunosPorTurma_MatchesSqlIncludingZeroEnrollments()
        {
            using (var server = TestServerExtensions.CreateApi())
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Escola"].ConnectionString))
            {
                connection.Open();
                var expected = (await connection.QueryAsync<RelatorioLinha>(@"
SELECT t.Nome AS NomeTurma,
       COUNT(m.Id) AS QuantidadeAlunos,
       t.VagasDisponiveis AS VagasRestantes
FROM dbo.Turma t
LEFT JOIN dbo.Matricula m ON m.TurmaId = t.Id
GROUP BY t.Id, t.Nome, t.VagasDisponiveis
ORDER BY t.Nome;")).ToList();

                var response = await server.GetJsonAsync("/api/relatorios/alunos-por-turma");
                var json = Assert.IsType<JArray>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(expected.Count, json.Count);
                Assert.Contains(json, item => (int)item["quantidadeAlunos"] == 0);

                for (var i = 0; i < expected.Count; i++)
                {
                    Assert.Equal(expected[i].NomeTurma, (string)json[i]["nomeTurma"]);
                    Assert.Equal(expected[i].QuantidadeAlunos, (int)json[i]["quantidadeAlunos"]);
                    Assert.Equal(expected[i].VagasRestantes, (int)json[i]["vagasRestantes"]);
                }
            }
        }

        private sealed class RelatorioLinha
        {
            public string NomeTurma { get; set; }

            public int QuantidadeAlunos { get; set; }

            public int VagasRestantes { get; set; }
        }
    }
}
