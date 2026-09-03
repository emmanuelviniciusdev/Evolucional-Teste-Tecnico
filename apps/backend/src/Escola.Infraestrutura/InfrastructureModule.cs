using System.Configuration;
using Autofac;
using Escola.Dominio;
using Escola.Dominio.Repositorios;
using Escola.Infraestrutura.Cache;
using Escola.Infraestrutura.Data;
using Escola.Infraestrutura.Health;
using StackExchange.Redis;

namespace Escola.Infraestrutura
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ConnectionFactory>().As<IConnectionFactory>().SingleInstance();
            builder.Register(CreateRedisConnection).As<IConnectionMultiplexer>().SingleInstance();
            builder.RegisterType<RedisCacheService>().As<ICacheService>().SingleInstance();
            builder.RegisterType<DependencyChecker>().As<IDependencyChecker>().SingleInstance();
            builder.RegisterType<AlunoRepository>().As<IAlunoRepository>().InstancePerLifetimeScope();
            builder.RegisterType<TurmaRepository>().As<ITurmaRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MatriculaRepository>().As<IMatriculaRepository>().InstancePerLifetimeScope();
            builder.RegisterType<RelatorioRepository>().As<IRelatorioRepository>().InstancePerLifetimeScope();
        }

        private static IConnectionMultiplexer CreateRedisConnection(IComponentContext context)
        {
            var endpoint = ConfigurationManager.AppSettings["Redis"];
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ConfigurationErrorsException("App setting 'Redis' is missing.");
            }

            return ConnectionMultiplexer.Connect(endpoint);
        }
    }
}
