using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DBMigrate
{
    public class ConnectionStringParts
    {
        public ConnectionStringParts(string connectionString)
        {
            List<string> items = connectionString.Split(';').ToList();

            foreach (var item in items)
            {
                var splitItems = item.Split('=');

                if (item.Contains("Host", StringComparison.OrdinalIgnoreCase))
                    Host = splitItems[1];
                if (item.Contains("User Id", StringComparison.OrdinalIgnoreCase))
                    User = splitItems[1];
                if (item.Contains("Password", StringComparison.OrdinalIgnoreCase))
                    Password = splitItems[1];
                if (item.Contains("Database", StringComparison.OrdinalIgnoreCase))
                    Database = splitItems[1];
                if (item.Contains("Port", StringComparison.OrdinalIgnoreCase))
                    Port = splitItems[1];
            }
        }

        public String Host { get; set; }
        public String User { get; set; }
        public String Port { get; set; }
        public String Database { get; set; }
        public String Password { get; set; }

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(Port))
                    return false;
                if (String.IsNullOrEmpty(Host))
                    return false;
                if (String.IsNullOrEmpty(User))
                    return false;
                if (String.IsNullOrEmpty(Database))
                    return false;
                // it's actually okay if the password is blank.
                //if (String.IsNullOrEmpty(Password))
                //    return true;

                return true;
            }

        }
    }
}
