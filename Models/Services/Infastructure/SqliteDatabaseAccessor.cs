using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace mvc.Models.Services.Infastructure
{
    public class SqliteDatabaseAccessor : IDatabaseAccessor
    {
        public DataSet Query(FormattableString fquery)
        {
            // la using automaticamente implementa la dispose sia in caso di errori che non
            
            // Creiamo dei SqliteParameter a partire dalla FormattableString
            var queryArguments = fquery.GetArguments(); // es trova 2 argomenti [0]=5 e [1]=5
            var sqliteParameters = new List<SqliteParameter>();
            for (var i = 0; i < queryArguments.Length; i++)
            {
                var parameter = new SqliteParameter(i.ToString(), queryArguments[i]);
                sqliteParameters.Add(parameter);
                queryArguments[i] = "@" + i; // l'argomento diventa es @0 @1
            }
            string query = fquery.ToString(); // ora la stringa ha parametri accettabili da sqlite (@0 e @1)

            using (var conn  = new SqliteConnection("Data Source=Data/MyCourse.db"))
            {
                conn.Open();
                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqliteParameters); // aggiunge i 2 parametri
                    using (var reader = cmd.ExecuteReader())
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
                        } while (!reader.IsClosed);


                        return ds;
                    }
                }
            }
        }
    }
}