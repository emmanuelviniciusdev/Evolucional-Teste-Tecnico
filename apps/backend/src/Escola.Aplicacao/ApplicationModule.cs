using Autofac;
using Escola.Aplicacao.Alunos;
using Escola.Aplicacao.Health;
using Escola.Aplicacao.Matriculas;
using Escola.Aplicacao.Relatorios;
using Escola.Aplicacao.Turmas;

namespace Escola.Aplicacao
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AlunoService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<TurmaService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<MatriculaService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<RelatorioService>().AsSelf().InstancePerLifetimeScope();
        }
    }
}
