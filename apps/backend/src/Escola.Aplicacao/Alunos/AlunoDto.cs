using System;
using Newtonsoft.Json;

namespace Escola.Aplicacao.Alunos
{
    /// <summary>
    /// Aluno returned by the enrollment API.
    /// </summary>
    public class AlunoDto
    {
        /// <summary>Generated identifier.</summary>
        public int Id { get; set; }

        /// <summary>Full name.</summary>
        public string Nome { get; set; }

        /// <summary>Complete email address.</summary>
        public string Email { get; set; }

        /// <summary>Birth date as YYYY-MM-DD.</summary>
        [JsonConverter(typeof(IsoDateOnlyConverter))]
        public DateTime DataNascimento { get; set; }

        /// <summary>Whether the aluno is active. Logical delete sets this to false.</summary>
        public bool Ativo { get; set; }

        /// <summary>When the aluno was registered.</summary>
        public DateTime DataCadastro { get; set; }
    }
}
