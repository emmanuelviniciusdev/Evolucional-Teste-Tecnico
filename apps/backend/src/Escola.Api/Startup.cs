using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Escola.Aplicacao;
using Escola.Api.Filters;
using Escola.Api.Swagger;
using Escola.Infraestrutura;
using Microsoft.Owin.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using Swashbuckle.Application;

[assembly: Microsoft.Owin.OwinStartup(typeof(Escola.Api.Startup))]

namespace Escola.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            var container = ConfigureHttp(config);
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
            app.UseStageMarker(PipelineStage.MapHandler);
        }

        public static IContainer ConfigureHttp(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.DateParseHandling = DateParseHandling.None;
            config.Filters.Add(new JsonExceptionFilter());
            ConfigureSwagger(config);

            var container = BuildContainer(config);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            return container;
        }

        private static void ConfigureSwagger(HttpConfiguration config)
        {
            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "Escola Enrollment API")
                    .Description("School enrollment API: alunos, turmas, matrículas, and SQL reports.");
                foreach (var xmlPath in DistinctExistingXmlCommentPaths())
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.OperationFilter<CopySummaryToDescriptionFilter>();
                c.SchemaFilter<IsoDateOnlySchemaFilter>();
            }).EnableSwaggerUi();

            config.Routes.MapHttpRoute(
                name: "swagger_root",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler(SwaggerRootUrl, "swagger/ui/index"));
        }

        private static string SwaggerRootUrl(HttpRequestMessage request)
        {
            return request.RequestUri.GetLeftPart(UriPartial.Authority);
        }

        private static IEnumerable<string> DistinctExistingXmlCommentPaths()
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var candidate in XmlCommentCandidates())
            {
                if (!File.Exists(candidate))
                {
                    continue;
                }

                var fullPath = Path.GetFullPath(candidate);
                if (seen.Add(fullPath))
                {
                    yield return fullPath;
                }
            }
        }

        private static IEnumerable<string> XmlCommentCandidates()
        {
            var fileNames = new[] { "Escola.Api.xml", "Escola.Aplicacao.xml" };
            foreach (var root in XmlCommentRoots())
            {
                if (string.IsNullOrWhiteSpace(root))
                {
                    continue;
                }

                foreach (var fileName in fileNames)
                {
                    yield return Path.Combine(root, fileName);
                }
            }
        }

        private static IEnumerable<string> XmlCommentRoots()
        {
            yield return AppDomain.CurrentDomain.BaseDirectory;
            yield return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");

            var apiDirectory = Path.GetDirectoryName(typeof(Startup).Assembly.Location);
            if (!string.IsNullOrEmpty(apiDirectory))
            {
                yield return apiDirectory;
            }

            var aplicacaoDirectory = Path.GetDirectoryName(typeof(ApplicationModule).Assembly.Location);
            if (!string.IsNullOrEmpty(aplicacaoDirectory))
            {
                yield return aplicacaoDirectory;
            }
        }

        private static IContainer BuildContainer(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterModule<ApplicationModule>();
            builder.RegisterModule<InfrastructureModule>();
            return builder.Build();
        }
    }
}
