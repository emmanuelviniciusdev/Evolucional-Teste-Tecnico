using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Escola.Aplicacao;
using Escola.Api.Filters;
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
                var bin = AppDomain.CurrentDomain.BaseDirectory;
                IncludeXmlCommentsIfPresent(c, Path.Combine(bin, "Escola.Api.xml"));
                IncludeXmlCommentsIfPresent(c, Path.Combine(bin, "Escola.Aplicacao.xml"));
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

        private static void IncludeXmlCommentsIfPresent(SwaggerDocsConfig config, string path)
        {
            if (File.Exists(path))
            {
                config.IncludeXmlComments(path);
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
