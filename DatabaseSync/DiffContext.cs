using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBMigrate;

namespace DatabaseSync
{
    public class DiffContext : IDBSyncContext
    {
        public DiffContext(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionStringParts = new ConnectionStringParts(ConnectionString);
        }

        public string ConnectionString { get; set; }
        public ConnectionStringParts ConnectionStringParts { get; set; }
        public string Name { get; set; }        
        public int VersionNumber { get; set; }

        public bool IsValid => !String.IsNullOrEmpty(Name) && VersionNumber > 0 && ConnectionStringParts.IsValid;
    }
}
