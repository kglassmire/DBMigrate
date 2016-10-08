using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Npgsql;
using NpgsqlTypes;

namespace DatabaseSync
{
    public class Publish
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private PublishContext _context;
        private SchemaVersion _currentSchemaVersion;
        private string _dumpFileName;
        private string _dumpFileText;
        private string _assemblyVersion = "1.0.0"; // THIS NEEDS TO BE PASSED IN SOMEHOW.

        public Publish(PublishContext context, string dumpFileName)
        {
            _context = context;
            _currentSchemaVersion = DetermineCurrentSchemaVersion();
            _dumpFileName = dumpFileName;
            _dumpFileText = File.ReadAllText(_dumpFileName);

            // if the NeedDatabaseCreation flag is set that means it was our first time so we're gonna publish anyway
            if (NeedDatabaseCreation || CurrentSchemaVersion.SchemaDump != _dumpFileText)
                PublishDump(_dumpFileText);
        }

        public bool NeedDatabaseCreation { get; set; }        
        public SchemaVersion CurrentSchemaVersion
        {
            get { return _currentSchemaVersion; }
            set { _currentSchemaVersion = value; }
        }


        public SchemaVersion DetermineCurrentSchemaVersion()
        {           
            if (!PublishSchemaExists())
            {
                logger.Info("Publish schema not detected. Creating schema now...");
                NeedDatabaseCreation = true;
                if (CreatePublishSchema())
                    logger.Info("Schema successfully created.");

                // We can return now because we don't have any schema versions to create. 
                // We can just go ahead and create the first schema version.
                return null;
            }

            logger.Info("Publish schema detected. Determining current schema version.");
            return QueryCurrentSchemaVersion();
        }

        private SchemaVersion QueryCurrentSchemaVersion()
        {
            SchemaVersion schemaVersion = new SchemaVersion();
            
            using (var conn = DbConnectionFactory.GetOpenConnection(_context))
            {
                var sql = @"
    SELECT * 
    FROM public.databasesync
    WHERE id = (select max(id) from databasesync)
;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            throw new Exception("There are no rows to determine the current version of the schema.");

                        schemaVersion.Id = (int) reader["id"];
                        schemaVersion.SchemaDump = reader["schemadump"].ToString();
                        schemaVersion.AssemblyVersion = reader["assemblyversion"].ToString();
                        schemaVersion.Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp"));
                    }
                }
            }

            return schemaVersion;
        }

        public bool PublishSchemaExists()
        {
            bool publishSchemaExists = false;
            using (var conn = DbConnectionFactory.GetOpenConnection(_context))
            {
                var sql = @"SELECT EXISTS(
    SELECT * 
    FROM information_schema.tables 
    WHERE 
      table_schema = @schema AND 
      table_name = @table
);";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("schema", NpgsqlDbType.Text, "public");
                    cmd.Parameters.AddWithValue("table", NpgsqlDbType.Text, "databasesync");
                    object result = cmd.ExecuteScalar();
                    publishSchemaExists = (bool) result;
                }
            }

            return publishSchemaExists;
        }

        public bool CreatePublishSchema()
        {
            int result = 0;
            using (var conn = DbConnectionFactory.GetOpenConnection(_context))
            {
                var sql = @"CREATE TABLE public.databasesync
(
id serial NOT NULL,
schemadump text NOT NULL,
timestamp timestamp without time zone NOT NULL,
assemblyversion text NOT NULL,
CONSTRAINT databasesync_pkey PRIMARY KEY (id)
);";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    result = (int)cmd.ExecuteNonQuery();
                }
            }

            return result != 0;
        }

        public int PublishDump(string dumpData, bool isFilePath = false)
        {
            if (isFilePath)
            {
                dumpData = File.ReadAllText(dumpData);
            }

            int result = 0;
            using (var conn = DbConnectionFactory.GetOpenConnection(_context))
            {
                var sql = @"INSERT INTO public.databasesync
(
schemadump,
timestamp,
assemblyversion
)
VALUES
(
@dumpData,
@timeStamp,
@assemblyVersion
)
;";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("dumpData", NpgsqlDbType.Text, dumpData);
                    cmd.Parameters.AddWithValue("timeStamp", NpgsqlDbType.Timestamp, DateTime.Now);
                    cmd.Parameters.AddWithValue("assemblyVersion", NpgsqlDbType.Text, _assemblyVersion);
                    result = (int)cmd.ExecuteNonQuery();
                }
            }

            return result;
        }
    }
}
