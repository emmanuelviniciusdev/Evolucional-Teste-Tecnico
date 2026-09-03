using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;

namespace Escola.Dominio.Repositorios
{
    public interface ITurmaRepository
    {
        Task<IReadOnlyList<Turma>> ListAsync();

        Task<Turma> GetByIdAsync(int id);

        Task<Turma> GetByIdForUpdateAsync(int id, IDbTransaction transaction);

        Task<int> TryDecrementVagasAsync(int id, IDbTransaction transaction);
    }
}
