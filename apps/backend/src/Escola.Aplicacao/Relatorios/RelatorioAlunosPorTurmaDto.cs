namespace Escola.Aplicacao.Relatorios
{
    /// <summary>
    /// Per-turma enrollment counts produced by a SQL JOIN/GROUP BY query.
    /// </summary>
    public class RelatorioAlunosPorTurmaDto
    {
        /// <summary>Turma name.</summary>
        public string NomeTurma { get; set; }

        /// <summary>Number of matrículas in that turma.</summary>
        public int QuantidadeAlunos { get; set; }

        /// <summary>Remaining seats (VagasDisponiveis).</summary>
        public int VagasRestantes { get; set; }
    }
}
