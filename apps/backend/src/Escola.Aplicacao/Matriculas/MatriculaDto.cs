using System;

namespace Escola.Aplicacao.Matriculas
{
    /// <summary>
    /// Created matrícula.
    /// </summary>
    public class MatriculaDto
    {
        /// <summary>Generated identifier.</summary>
        public int Id { get; set; }

        /// <summary>Enrolled aluno.</summary>
        public int AlunoId { get; set; }

        /// <summary>Target turma.</summary>
        public int TurmaId { get; set; }

        /// <summary>When the matrícula was stored.</summary>
        public DateTime DataMatricula { get; set; }
    }
}
