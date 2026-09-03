namespace Escola.Aplicacao.Matriculas
{
    /// <summary>
    /// Body used to enroll an aluno in a turma.
    /// </summary>
    public class CriarMatriculaDto
    {
        /// <summary>Existing aluno identifier.</summary>
        public int AlunoId { get; set; }

        /// <summary>Existing turma identifier.</summary>
        public int TurmaId { get; set; }
    }
}
