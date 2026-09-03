using System;

namespace Escola.Dominio.Excecoes
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
