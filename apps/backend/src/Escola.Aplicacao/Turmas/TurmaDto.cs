namespace Escola.Aplicacao.Turmas
{
    /// <summary>
    /// Turma with remaining seats from the stored VagasDisponiveis value.
    /// </summary>
    public class TurmaDto
    {
        /// <summary>Turma identifier.</summary>
        public int Id { get; set; }

        /// <summary>Turma name.</summary>
        public string Nome { get; set; }

        /// <summary>Period: Manha, Tarde, or Noite.</summary>
        public string Periodo { get; set; }

        /// <summary>Total seats.</summary>
        public int VagasTotal { get; set; }

        /// <summary>Remaining seats stored in the database.</summary>
        public int VagasDisponiveis { get; set; }
    }
}
