using System.Data;
using System.Threading.Tasks;
using Escola.Dominio.Entidades;

namespace Escola.Dominio.Repositorios
{
    public interface IMatriculaRepository
    {
        Task<bool> ExistsAsync(int alunoId, int turmaId, IDbTransaction transaction);

        Task<Matricula> InsertAsync(int alunoId, int turmaId, IDbTransaction transaction);
    }
}
