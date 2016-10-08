using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBMigrate;
using Npgsql;

namespace DatabaseSync
{
    public class DbConnectionFactory
    {
        public static NpgsqlConnection GetOpenConnection(IDBSyncContext context)
        {
            var cnn = CreateNpgSqlConnection(context.ConnectionString);
            cnn.Open();
            return cnn;
        }

        public static NpgsqlConnection GetOpenConnection(String connectionString)
        {
            var cnn = CreateNpgSqlConnection(connectionString);
            cnn.Open();
            return cnn;
        }

        private static NpgsqlConnection CreateNpgSqlConnection(String connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

    }
}
