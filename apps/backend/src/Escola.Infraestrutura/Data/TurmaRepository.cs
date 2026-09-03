using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Data
{
    public class TurmaRepository : ITurmaRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public TurmaRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Turma>> ListAsync()
        {
            const string sql = @"
SELECT Id, Nome, Periodo, VagasTotal, VagasDisponiveis
FROM dbo.Turma
ORDER BY Nome, Id;";

            using (var connection = _connectionFactory.Create())
            {
                var rows = await connection.QueryAsync<Turma>(sql).ConfigureAwait(false);
                return rows.AsList();
            }
        }

        public async Task<Turma> GetByIdAsync(int id)
        {
            const string sql = @"
SELECT Id, Nome, Periodo, VagasTotal, VagasDisponiveis
FROM dbo.Turma
WHERE Id = @Id;";

            using (var connection = _connectionFactory.Create())
            {
                return await connection.QuerySingleOrDefaultAsync<Turma>(sql, new { Id = id }).ConfigureAwait(false);
            }
        }

        public async Task<Turma> GetByIdForUpdateAsync(int id, IDbTransaction transaction)
        {
            const string sql = @"
SELECT Id, Nome, Periodo, VagasTotal, VagasDisponiveis
FROM dbo.Turma WITH (UPDLOCK, ROWLOCK)
WHERE Id = @Id;";

            return await transaction.Connection.QuerySingleOrDefaultAsync<Turma>(
                sql,
                new { Id = id },
                transaction).ConfigureAwait(false);
        }

        public async Task<int> TryDecrementVagasAsync(int id, IDbTransaction transaction)
        {
            const string sql = @"
UPDATE dbo.Turma
SET VagasDisponiveis = VagasDisponiveis - 1
WHERE Id = @Id AND VagasDisponiveis > 0;";

            return await transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction).ConfigureAwait(false);
        }
    }
}
