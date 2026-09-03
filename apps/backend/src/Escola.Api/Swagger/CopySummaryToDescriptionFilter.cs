using System.Web.Http.Description;
using Swashbuckle.Swagger;

namespace Escola.Api.Swagger
{
    internal sealed class CopySummaryToDescriptionFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(operation.description) && !string.IsNullOrWhiteSpace(operation.summary))
            {
                operation.description = operation.summary;
            }
        }
    }
}
