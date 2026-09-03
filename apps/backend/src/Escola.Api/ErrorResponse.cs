using Newtonsoft.Json;

namespace Escola.Api
{
    /// <summary>
    /// JSON error body used for HTTP 400, 404, 409, and 500.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>Human-readable message. Assignment-route errors are pt-BR.</summary>
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}
