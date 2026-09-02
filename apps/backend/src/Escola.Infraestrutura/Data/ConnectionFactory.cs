using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Escola.Dominio;

namespace Escola.Infraestrutura.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        public IDbConnection Create()
        {
            var settings = ConfigurationManager.ConnectionStrings["Escola"];
            if (settings == null || string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ConfigurationErrorsException("Connection string 'Escola' is missing.");
            }

            var connection = new SqlConnection(settings.ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
