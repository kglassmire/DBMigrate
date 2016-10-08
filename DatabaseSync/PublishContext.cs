using System;
using DBMigrate;

namespace DatabaseSync
{
    public class PublishContext : IDBSyncContext
    {
        public PublishContext(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionStringParts = new ConnectionStringParts(ConnectionString);
        }

        public string ConnectionString { get; set; }
        public ConnectionStringParts ConnectionStringParts { get; set; }
        public string Name { get; set; }

        public bool IsValid => !string.IsNullOrEmpty(Name) && ConnectionStringParts.IsValid;
    }
}