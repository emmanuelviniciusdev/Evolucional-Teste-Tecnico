using Xunit;

namespace Escola.Testes.Integracao.Infra
{
    [CollectionDefinition("EnrollmentApi")]
    public class EnrollmentApiCollection : ICollectionFixture<EnrollmentDatabaseFixture>
    {
    }
}
