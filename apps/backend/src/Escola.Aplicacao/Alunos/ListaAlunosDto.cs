using System.Collections.Generic;

namespace Escola.Aplicacao.Alunos
{
    /// <summary>
    /// Paginated aluno list including the total matching the optional name filter.
    /// </summary>
    public class ListaAlunosDto
    {
        /// <summary>Alunos on the current page (active and inactive).</summary>
        public IReadOnlyList<AlunoDto> Alunos { get; set; }

        /// <summary>Total records matching the filter, not only this page.</summary>
        public int Total { get; set; }

        /// <summary>1-based page that was returned.</summary>
        public int Pagina { get; set; }

        /// <summary>Page size that was used.</summary>
        public int TamanhoPagina { get; set; }
    }
}
