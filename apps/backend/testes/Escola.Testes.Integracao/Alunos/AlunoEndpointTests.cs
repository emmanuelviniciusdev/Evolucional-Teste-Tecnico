using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Escola.Testes.Integracao.Infra;
using Microsoft.Owin.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Escola.Testes.Integracao.Alunos
{
    [Collection("EnrollmentApi")]
    public class AlunoEndpointTests
    {
        public AlunoEndpointTests(EnrollmentDatabaseFixture fixture)
        {
            fixture.Reset();
        }

        [Fact]
        public async Task GetAlunos_NoQuery_ReturnsFirstPageAndTotal()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.GetJsonAsync("/api/alunos");
                var json = Assert.IsType<JObject>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, (int)json["pagina"]);
                Assert.Equal(10, (int)json["tamanhoPagina"]);
                Assert.Equal(7, (int)json["total"]);
                var alunos = json["alunos"].Value<JArray>();
                Assert.Equal(7, alunos.Count);
                foreach (var aluno in alunos)
                {
                    Assert.True((bool)aluno["ativo"]);
                    Assert.NotEqual(4, (int)aluno["id"]);
                }
            }
        }

        [Fact]
        public async Task GetAlunos_FilterByName_ReturnsMatchingTotal()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.GetJsonAsync("/api/alunos?nome=ana");
                var json = Assert.IsType<JObject>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(1, (int)json["total"]);
                Assert.Contains("ana", ((string)json["alunos"][0]["nome"]), StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public async Task GetAlunos_FilterByInactiveName_ReturnsEmpty()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.GetJsonAsync("/api/alunos?nome=diego");
                var json = Assert.IsType<JObject>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(0, (int)json["total"]);
                Assert.Empty(json["alunos"].Value<JArray>());
            }
        }

        [Fact]
        public async Task GetAluno_InactiveId_ReturnsNotFound()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.GetJsonAsync("/api/alunos/4");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task GetAluno_MissingId_ReturnsNotFound()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.GetJsonAsync("/api/alunos/99999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostAluno_CompleteEmailWithoutDot_ReturnsCreated()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/alunos", new
                {
                    nome = "Ana Unique",
                    email = "anasouza2345@email.com",
                    dataNascimento = "2006-03-14"
                });
                var json = Assert.IsType<JObject>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Contains("/api/alunos/" + (int)json["id"], response.Headers.Location.ToString());
                Assert.Equal("anasouza2345@email.com", (string)json["email"]);
                Assert.Equal("2006-03-14", (string)json["dataNascimento"]);
                Assert.True((bool)json["ativo"]);
            }
        }

        [Theory]
        [InlineData("a@b")]
        [InlineData("user@localhost")]
        public async Task PostAluno_IncompleteEmail_ReturnsBadRequest(string email)
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/alunos", new
                {
                    nome = "Invalido",
                    email,
                    dataNascimento = "2006-03-14"
                });

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task PostAluno_DataNascimentoWithTime_ReturnsBadRequest()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/alunos", new
                {
                    nome = "Invalido",
                    email = "valido@email.com",
                    dataNascimento = "2006-03-14T00:00:00"
                });

                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task PutAluno_ValidBody_ReturnsUpdatedFields()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PutJsonAsync("/api/alunos/1", new
                {
                    nome = "Ana Atualizada",
                    email = "ana.atualizada@email.com",
                    dataNascimento = "2006-03-14"
                });
                var json = Assert.IsType<JObject>(await response.ReadJsonAsync());

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Ana Atualizada", (string)json["nome"]);
                Assert.Equal("ana.atualizada@email.com", (string)json["email"]);
                Assert.True((bool)json["ativo"]);
            }
        }

        [Fact]
        public async Task DeleteAluno_LogicalDelete_OmittedFromGetEndpoints()
        {
            using (var server = TestServerExtensions.CreateApi())
            {
                var delete = await server.HttpClient.DeleteAsync("/api/alunos/1");
                Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

                var get = await server.GetJsonAsync("/api/alunos/1");
                Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);

                var list = await server.GetJsonAsync("/api/alunos");
                var json = Assert.IsType<JObject>(await list.ReadJsonAsync());
                Assert.Equal(HttpStatusCode.OK, list.StatusCode);
                Assert.Equal(6, (int)json["total"]);
                foreach (var aluno in json["alunos"].Value<JArray>())
                {
                    Assert.NotEqual(1, (int)aluno["id"]);
                }
            }
        }

        [Fact]
        public async Task PostAluno_DoesNotInsertIntoApiDatabase()
        {
            var email = "isolado." + Guid.NewGuid().ToString("N") + "@email.com";
            using (var server = TestServerExtensions.CreateApi())
            {
                var response = await server.PostJsonAsync("/api/alunos", new
                {
                    nome = "Isolado",
                    email,
                    dataNascimento = "2006-03-14"
                });
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }

            var apiCs = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["Escola"].ConnectionString)
            {
                InitialCatalog = "TesteEscola"
            };

            using (var connection = new SqlConnection(apiCs.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(1) FROM dbo.Aluno WHERE Email = @Email", connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    Assert.Equal(0, (int)command.ExecuteScalar());
                }
            }
        }
    }
}
