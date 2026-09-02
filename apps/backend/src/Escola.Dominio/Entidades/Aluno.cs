using System;

namespace Escola.Dominio.Entidades
{
    /// <summary>
    /// Representa um aluno da escola.
    /// </summary>
    public class Aluno
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public DateTime DataNascimento { get; set; }

        public bool Ativo { get; set; }

        public DateTime DataCadastro { get; set; }
    }
}
