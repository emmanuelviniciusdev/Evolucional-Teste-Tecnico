using Autofac;
using Escola.Aplicacao.Health;

namespace Escola.Aplicacao
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
