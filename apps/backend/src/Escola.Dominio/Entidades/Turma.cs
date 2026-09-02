namespace Escola.Dominio.Entidades
{
    /// <summary>
    /// Representa uma turma da escola.
    /// </summary>
    public class Turma
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        /// <summary>
        /// Período da turma: Manha, Tarde ou Noite.
        /// </summary>
        public string Periodo { get; set; }

        public int VagasTotal { get; set; }

        public int VagasDisponiveis { get; set; }
    }
}
