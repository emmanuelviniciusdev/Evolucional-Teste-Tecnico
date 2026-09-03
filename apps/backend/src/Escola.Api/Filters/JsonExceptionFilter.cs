using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Escola.Dominio.Excecoes;

namespace Escola.Api.Filters
{
    public class JsonExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is ValidationException)
            {
                context.Response = context.Request.CreateResponse(
                    HttpStatusCode.BadRequest,
                    new ErrorResponse { Error = context.Exception.Message });
                return;
            }

            if (context.Exception is NotFoundException)
            {
                context.Response = context.Request.CreateResponse(
                    HttpStatusCode.NotFound,
                    new ErrorResponse { Error = context.Exception.Message });
                return;
            }

            if (context.Exception is ConflictException)
            {
                context.Response = context.Request.CreateResponse(
                    HttpStatusCode.Conflict,
                    new ErrorResponse { Error = context.Exception.Message });
                return;
            }

            context.Response = context.Request.CreateResponse(
                HttpStatusCode.InternalServerError,
                new ErrorResponse { Error = "An unexpected error occurred." });
        }
    }
}
