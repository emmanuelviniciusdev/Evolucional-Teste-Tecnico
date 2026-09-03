using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Escola.Dominio.Repositorios;

namespace Escola.Aplicacao.Relatorios
{
    public class RelatorioService
    {
        private readonly IRelatorioRepository _relatorioRepository;

        public RelatorioService(IRelatorioRepository relatorioRepository)
        {
            _relatorioRepository = relatorioRepository;
        }

        public async Task<IReadOnlyList<RelatorioAlunosPorTurmaDto>> ListarAlunosPorTurmaAsync()
        {
            var rows = await _relatorioRepository.ListAlunosPorTurmaAsync().ConfigureAwait(false);
            return rows.Select(row => new RelatorioAlunosPorTurmaDto
            {
                NomeTurma = row.NomeTurma,
                QuantidadeAlunos = row.QuantidadeAlunos,
                VagasRestantes = row.VagasRestantes
            }).ToList();
        }
    }
}
