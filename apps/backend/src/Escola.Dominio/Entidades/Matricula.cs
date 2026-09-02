using System;

namespace Escola.Dominio.Entidades
{
    /// <summary>
    /// Representa a matrícula de um aluno em uma turma.
    /// </summary>
    public class Matricula
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }

        public int TurmaId { get; set; }

        public DateTime DataMatricula { get; set; }
    }
}
