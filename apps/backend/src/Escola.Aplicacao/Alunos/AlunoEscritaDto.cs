namespace Escola.Aplicacao.Alunos
{
    /// <summary>
    /// Body used to create or update an aluno. Birth date must be YYYY-MM-DD.
    /// </summary>
    public class AlunoEscritaDto
    {
        /// <summary>Full name (required, max 120 characters).</summary>
        public string Nome { get; set; }

        /// <summary>
        /// Complete email (required, max 120 characters). Local-part may include a dot
        /// (ana.souza@email.com) or only letters and digits (anasouza2345@email.com).
        /// </summary>
        public string Email { get; set; }

        /// <summary>Calendar date only, format YYYY-MM-DD (for example 2006-03-14).</summary>
        public string DataNascimento { get; set; }
    }
}
