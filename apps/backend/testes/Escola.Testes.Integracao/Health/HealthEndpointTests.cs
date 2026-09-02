using System.Net;
using System.Threading.Tasks;
using Escola.Api;
using Microsoft.Owin.Testing;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Escola.Testes.Integracao.Health
{
    public class HealthEndpointTests
    {
        [Fact]
        public async Task GetHealth_WhenDependenciesAreReachable_ReturnsOk()
        {
            using (var server = TestServer.Create<Startup>())
            {
                var response = await server.HttpClient.GetAsync("/api/health");
                var body = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(body);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
                Assert.Equal("healthy", (string)json["status"]);
                Assert.Equal("healthy", (string)json["dependencies"]["sqlServer"]);
                Assert.Equal("healthy", (string)json["dependencies"]["redis"]);
            }
        }
    }
}
