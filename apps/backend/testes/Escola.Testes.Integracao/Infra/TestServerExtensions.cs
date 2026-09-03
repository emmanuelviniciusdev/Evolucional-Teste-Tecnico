using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Escola.Api;
using Microsoft.Owin.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Escola.Testes.Integracao.Infra
{
    internal static class TestServerExtensions
    {
        public static TestServer CreateApi()
        {
            return TestServer.Create<Startup>();
        }

        public static async Task<HttpResponseMessage> GetJsonAsync(this TestServer server, string path)
        {
            return await server.HttpClient.GetAsync(path);
        }

        public static async Task<HttpResponseMessage> PostJsonAsync(this TestServer server, string path, object body)
        {
            return await server.HttpClient.PostAsync(path, ToJsonContent(body));
        }

        public static async Task<HttpResponseMessage> PutJsonAsync(this TestServer server, string path, object body)
        {
            return await server.HttpClient.PutAsync(path, ToJsonContent(body));
        }

        public static async Task<JToken> ReadJsonAsync(this HttpResponseMessage response)
        {
            var text = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            return JToken.Parse(text);
        }

        private static StringContent ToJsonContent(object body)
        {
            var json = body as string ?? JsonConvert.SerializeObject(body);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
