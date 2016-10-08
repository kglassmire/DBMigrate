using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseSync;

namespace DBMigrate
{
    public interface IDBSyncContext
    {
        ConnectionStringParts ConnectionStringParts { get; set; }
        string ConnectionString { get; set; }
        string Name { get; set; }

        bool IsValid { get; }
    }
}
