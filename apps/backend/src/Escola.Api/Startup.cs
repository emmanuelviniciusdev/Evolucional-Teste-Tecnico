using System;
using System.IO;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Escola.Aplicacao;
using Escola.Api.Filters;
using Escola.Infraestrutura;
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
            ConfigureWebApi(config);
            var container = BuildContainer(config);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }

        private static void ConfigureWebApi(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.DateParseHandling = DateParseHandling.None;
            config.Filters.Add(new JsonExceptionFilter());
            ConfigureSwagger(config);
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
