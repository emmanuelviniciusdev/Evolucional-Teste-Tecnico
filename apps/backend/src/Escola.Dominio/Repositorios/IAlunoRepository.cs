using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;

namespace Escola.Dominio.Repositorios
{
    public interface IAlunoRepository
    {
        Task<IReadOnlyList<Aluno>> ListAsync(string nome, int offset, int take);

        Task<int> CountAsync(string nome);

        Task<Aluno> GetByIdAsync(int id, IDbTransaction transaction = null);

        Task<int> InsertAsync(Aluno aluno);

        Task UpdateAsync(Aluno aluno);

        Task LogicalDeleteAsync(int id);
    }
}
