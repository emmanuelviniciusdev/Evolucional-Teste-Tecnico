using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio;
using Escola.Dominio.Relatorios;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Data
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public RelatorioRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<AlunosPorTurma>> ListAlunosPorTurmaAsync()
        {
            const string sql = @"
SELECT t.Nome AS NomeTurma,
       COUNT(m.Id) AS QuantidadeAlunos,
       t.VagasDisponiveis AS VagasRestantes
FROM dbo.Turma t
LEFT JOIN dbo.Matricula m ON m.TurmaId = t.Id
GROUP BY t.Id, t.Nome, t.VagasDisponiveis
ORDER BY t.Nome;";

            using (var connection = _connectionFactory.Create())
            {
                var rows = await connection.QueryAsync<AlunosPorTurma>(sql).ConfigureAwait(false);
                return rows.AsList();
            }
        }
    }
}
