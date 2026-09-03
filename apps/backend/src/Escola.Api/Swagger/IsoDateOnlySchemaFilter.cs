using System;
using System.Reflection;
using Escola.Aplicacao;
using Newtonsoft.Json;
using Swashbuckle.Swagger;

namespace Escola.Api.Swagger
{
    internal sealed class IsoDateOnlySchemaFilter : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            if (schema == null || schema.properties == null || type == null)
            {
                return;
            }

            foreach (var property in type.GetProperties())
            {
                if (!IsDateOnly(property))
                {
                    continue;
                }

                Schema propertySchema;
                if (!TryGetPropertySchema(schema, property.Name, out propertySchema))
                {
                    continue;
                }

                propertySchema.type = "string";
                propertySchema.format = "date";
                propertySchema.example = "2006-03-14";
            }
        }

        private static bool IsDateOnly(PropertyInfo property)
        {
            if (string.Equals(property.Name, "DataNascimento", StringComparison.Ordinal))
            {
                return true;
            }

            var converter = property.GetCustomAttribute<JsonConverterAttribute>();
            return converter != null && converter.ConverterType == typeof(IsoDateOnlyConverter);
        }

        private static bool TryGetPropertySchema(Schema schema, string clrName, out Schema propertySchema)
        {
            var camelCase = ToCamelCase(clrName);
            if (schema.properties.TryGetValue(camelCase, out propertySchema))
            {
                return true;
            }

            return schema.properties.TryGetValue(clrName, out propertySchema);
        }

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
            {
                return name;
            }

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}
