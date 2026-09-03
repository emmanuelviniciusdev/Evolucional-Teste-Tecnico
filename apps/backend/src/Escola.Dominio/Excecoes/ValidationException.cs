using System;

namespace Escola.Dominio.Excecoes
{
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message)
        {
        }
    }
}
