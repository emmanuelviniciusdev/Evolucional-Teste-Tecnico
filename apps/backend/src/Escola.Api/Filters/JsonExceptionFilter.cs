using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Escola.Api.Filters
{
    public class JsonExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new { error = "An unexpected error occurred." });
        }
    }
}
