using System.Threading.Tasks;

namespace Escola.Dominio
{
    public interface IDependencyChecker
    {
        Task<bool> CanReachSqlServerAsync();

        Task<bool> CanReachRedisAsync();
    }
}
