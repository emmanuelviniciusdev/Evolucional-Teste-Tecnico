using System.Collections.Generic;
using System.Threading.Tasks;
using Escola.Dominio.Relatorios;

namespace Escola.Dominio.Repositorios
{
    public interface IRelatorioRepository
    {
        Task<IReadOnlyList<AlunosPorTurma>> ListAlunosPorTurmaAsync();
    }
}
