using System.Threading.Tasks;
using Escola.Aplicacao.Health;
using Escola.Dominio;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Escola.Testes.Unitarios.Health
{
    public class HealthServiceTests
    {
        [Fact]
        public async Task CheckAsync_WhenBothDependenciesAreReachable_ReportsHealthy()
        {
            var checker = Substitute.For<IDependencyChecker>();
            checker.CanReachSqlServerAsync().Returns(Task.FromResult(true));
            checker.CanReachRedisAsync().Returns(Task.FromResult(true));

            var sut = new HealthService(checker);

            var result = await sut.CheckAsync();

            result.Status.Should().Be(HealthReport.Healthy);
            result.Dependencies["sqlServer"].Should().Be(HealthReport.Healthy);
            result.Dependencies["redis"].Should().Be(HealthReport.Healthy);
            result.IsHealthy().Should().BeTrue();
        }

        [Fact]
        public async Task CheckAsync_WhenRedisIsUnreachable_ReportsUnavailable()
        {
            var checker = Substitute.For<IDependencyChecker>();
            checker.CanReachSqlServerAsync().Returns(Task.FromResult(true));
            checker.CanReachRedisAsync().Returns(Task.FromResult(false));

            var sut = new HealthService(checker);

            var result = await sut.CheckAsync();

            result.Status.Should().Be(HealthReport.Unavailable);
            result.Dependencies["sqlServer"].Should().Be(HealthReport.Healthy);
            result.Dependencies["redis"].Should().Be(HealthReport.Unavailable);
            result.IsHealthy().Should().BeFalse();
        }
    }
}
