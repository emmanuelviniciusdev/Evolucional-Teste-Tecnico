using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Escola.Dominio.Entidades;
using Escola.Dominio.Excecoes;
using Escola.Dominio.Repositorios;

namespace Escola.Infraestrutura.Data
{
    public class MatriculaRepository : IMatriculaRepository
    {
        public async Task<bool> ExistsAsync(int alunoId, int turmaId, IDbTransaction transaction)
        {
            const string sql = @"
SELECT CASE WHEN EXISTS (
    SELECT 1
    FROM dbo.Matricula
    WHERE AlunoId = @AlunoId AND TurmaId = @TurmaId
) THEN 1 ELSE 0 END;";

            return await transaction.Connection.ExecuteScalarAsync<bool>(
                sql,
                new { AlunoId = alunoId, TurmaId = turmaId },
                transaction).ConfigureAwait(false);
        }

        public async Task<Matricula> InsertAsync(int alunoId, int turmaId, IDbTransaction transaction)
        {
            const string sql = @"
INSERT INTO dbo.Matricula (AlunoId, TurmaId)
OUTPUT INSERTED.Id, INSERTED.AlunoId, INSERTED.TurmaId, INSERTED.DataMatricula
VALUES (@AlunoId, @TurmaId);";

            try
            {
                return await transaction.Connection.QuerySingleAsync<Matricula>(
                    sql,
                    new { AlunoId = alunoId, TurmaId = turmaId },
                    transaction).ConfigureAwait(false);
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                throw new ConflictException("Aluno já está matriculado nesta turma.");
            }
        }
    }
}
