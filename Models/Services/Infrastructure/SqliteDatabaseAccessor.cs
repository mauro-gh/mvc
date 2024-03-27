using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using mvc.Models.Options;
using mvc.Models.ValueObjects;

namespace mvc.Models.Services.Infrastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        private readonly ILogger<SqliteDatabaseAccessor> logger;
        private readonly IOptionsMonitor<ConnectionStringsOptions> connectionStringsOptions;

        // costruttore per ricevere la sezione del config (e ricevere gli aggiornamenti)
        public SqliteDatabaseAccessor(
                    ILogger<SqliteDatabaseAccessor> logger,
                    IOptionsMonitor<ConnectionStringsOptions> connectionStringsOptions)
        {
            this.logger = logger;
            this.connectionStringsOptions = connectionStringsOptions;
        }

        public async Task<DataSet> QueryAsync(FormattableString fquery)
        {

            // log strutturato
            logger.LogInformation(fquery.Format, fquery.GetArguments());


            // la using automaticamente implementa la dispose sia in caso di errori che non
            
            // Creiamo dei SqliteParameter a partire dalla FormattableString
            var queryArguments = fquery.GetArguments(); // es trova 2 argomenti [0]=5 e [1]=5
            var sqliteParameters = new List<SqliteParameter>();
            for (var i = 0; i < queryArguments.Length; i++)
            {
                // salto creazione sqlparameter per order by
                if (queryArguments[i] is Sql)
                {
                    continue;
                }
                var parameter = new SqliteParameter(i.ToString(), queryArguments[i]);
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i; // l'argomento diventa es @0 @1
            }
            string query = fquery.ToString(); // ora la stringa ha parametri accettabili da sqlite (@0 e @1)

            string connectionString = connectionStringsOptions.CurrentValue.Default;


            using (var conn  = new SqliteConnection(connectionString))
            {
                //conn.Open();
                await conn.OpenAsync();

                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqliteParameters); // aggiunge i 2 parametri
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        DataSet ds = new DataSet();
                        var i = 0;

                        // Il reader puo' contenere piu' datatable, quindi inserisco in un ciclo
                        do
                        {
                            i++;
                            DataTable dt = new DataTable($"Tabella {i}");
                            ds.Tables.Add(dt);
                            dt.Load(reader); 
                            //logger.LogInformation("Rows:{0}", dt.Rows.Count);                           
                        } while (!reader.IsClosed);


                        return ds;
                    }
                }
            }
        }
    }
}