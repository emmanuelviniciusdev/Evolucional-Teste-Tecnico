using System.Data;

namespace Escola.Dominio
{
    public interface IConnectionFactory
    {
        IDbConnection Create();
    }
}
