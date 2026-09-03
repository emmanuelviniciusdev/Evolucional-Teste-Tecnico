using System;

namespace Escola.Aplicacao
{
    public static class CacheKeys
    {
        public const string TurmasListagem = "turmas:listagem";

        public static readonly TimeSpan TurmasListagemTtl = TimeSpan.FromMinutes(5);
    }
}
