using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;

namespace Escola.Dominio.Repositorios
{
    public interface IAlunoRepository
    {
        /// <summary>Paged active alunos only, with optional name filter.</summary>
        Task<IReadOnlyList<Aluno>> ListAsync(string nome, int offset, int take);

        /// <summary>Count of active alunos matching the optional name filter.</summary>
        Task<int> CountAsync(string nome);

        /// <summary>Returns the aluno by id including inactive rows, or null when missing.</summary>
        Task<Aluno> GetByIdAsync(int id, IDbTransaction transaction = null);

        Task<int> InsertAsync(Aluno aluno);

        Task UpdateAsync(Aluno aluno);

        Task LogicalDeleteAsync(int id);
    }
}
