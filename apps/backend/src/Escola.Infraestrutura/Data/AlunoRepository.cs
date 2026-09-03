using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio;
using Escola.Dominio.Entidades;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Data
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly IConnectionFactory _connectionFactory;

        public AlunoRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IReadOnlyList<Aluno>> ListAsync(string nome, int offset, int take)
        {
            const string sql = @"
SELECT Id, Nome, Email, DataNascimento, Ativo, DataCadastro
FROM dbo.Aluno
WHERE (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%')
ORDER BY Nome, Id
OFFSET @Offset ROWS FETCH NEXT @Take ROWS ONLY;";

            using (var connection = _connectionFactory.Create())
            {
                var rows = await connection.QueryAsync<Aluno>(
                    sql,
                    new { Nome = nome, Offset = offset, Take = take }).ConfigureAwait(false);
                return rows.AsList();
            }
        }

        public async Task<int> CountAsync(string nome)
        {
            const string sql = @"
SELECT COUNT(1)
FROM dbo.Aluno
WHERE (@Nome IS NULL OR Nome LIKE '%' + @Nome + '%');";

            using (var connection = _connectionFactory.Create())
            {
                return await connection.ExecuteScalarAsync<int>(sql, new { Nome = nome }).ConfigureAwait(false);
            }
        }

        public async Task<Aluno> GetByIdAsync(int id, IDbTransaction transaction = null)
        {
            const string sql = @"
SELECT Id, Nome, Email, DataNascimento, Ativo, DataCadastro
FROM dbo.Aluno
WHERE Id = @Id;";

            if (transaction != null)
            {
                return await transaction.Connection.QuerySingleOrDefaultAsync<Aluno>(
                    sql,
                    new { Id = id },
                    transaction).ConfigureAwait(false);
            }

            using (var connection = _connectionFactory.Create())
            {
                return await connection.QuerySingleOrDefaultAsync<Aluno>(sql, new { Id = id }).ConfigureAwait(false);
            }
        }

        public async Task<int> InsertAsync(Aluno aluno)
        {
            const string sql = @"
INSERT INTO dbo.Aluno (Nome, Email, DataNascimento, Ativo)
OUTPUT INSERTED.Id
VALUES (@Nome, @Email, @DataNascimento, @Ativo);";

            using (var connection = _connectionFactory.Create())
            {
                return await connection.QuerySingleAsync<int>(sql, aluno).ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(Aluno aluno)
        {
            const string sql = @"
UPDATE dbo.Aluno
SET Nome = @Nome, Email = @Email, DataNascimento = @DataNascimento
WHERE Id = @Id;";

            using (var connection = _connectionFactory.Create())
            {
                await connection.ExecuteAsync(sql, aluno).ConfigureAwait(false);
            }
        }

        public async Task LogicalDeleteAsync(int id)
        {
            const string sql = @"UPDATE dbo.Aluno SET Ativo = 0 WHERE Id = @Id;";

            using (var connection = _connectionFactory.Create())
            {
                await connection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
            }
        }
    }
}
