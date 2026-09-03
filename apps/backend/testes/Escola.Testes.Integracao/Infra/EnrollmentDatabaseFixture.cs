using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using StackExchange.Redis;

namespace Escola.Testes.Integracao.Infra
{
    public class EnrollmentDatabaseFixture
    {
        public void Reset()
        {
            ApplySchemaAndSeed();
            FlushRedisDatabase1();
        }

        private static void ApplySchemaAndSeed()
        {
            var escola = ConfigurationManager.ConnectionStrings["Escola"];
            if (escola == null || string.IsNullOrWhiteSpace(escola.ConnectionString))
            {
                throw new InvalidOperationException("Connection string 'Escola' is missing.");
            }

            var builder = new SqlConnectionStringBuilder(escola.ConnectionString);
            if (!string.Equals(builder.InitialCatalog, "TesteEscola_Testes", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Integration tests must use database TesteEscola_Testes.");
            }

            var script = File.ReadAllText(FindInitSql()).Replace("TesteEscola", "TesteEscola_Testes");
            builder.InitialCatalog = "master";

            using (var connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                foreach (var batch in SplitGoBatches(script))
                {
                    using (var command = new SqlCommand(batch, connection))
                    {
                        command.CommandTimeout = 60;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void FlushRedisDatabase1()
        {
            var endpoint = ConfigurationManager.AppSettings["Redis"];
            using (var mux = ConnectionMultiplexer.Connect(endpoint))
            {
                mux.GetDatabase().Execute("FLUSHDB");
            }
        }

        private static string FindInitSql()
        {
            var fromOutput = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "init.sql");
            if (File.Exists(fromOutput))
            {
                return fromOutput;
            }

            throw new FileNotFoundException("Could not find init.sql next to the integration test assembly.", fromOutput);
        }

        private static IEnumerable<string> SplitGoBatches(string script)
        {
            var batches = new List<string>();
            var current = new StringBuilder();
            using (var reader = new StringReader(script))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.Equals(line.Trim(), "GO", StringComparison.OrdinalIgnoreCase))
                    {
                        var batch = current.ToString().Trim();
                        if (batch.Length > 0)
                        {
                            batches.Add(batch);
                        }

                        current.Clear();
                    }
                    else
                    {
                        current.AppendLine(line);
                    }
                }
            }

            var last = current.ToString().Trim();
            if (last.Length > 0)
            {
                batches.Add(last);
            }

            return batches;
        }
    }
}
