using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseSync;
using NLog;

namespace DatabaseSyncConsole
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            Dump dbDump = new Dump(ConfigurationManager.ConnectionStrings["local"].ConnectionString,
            ConfigurationManager.ConnectionStrings["local"].Name);
            logger.Info("Beginning DBDump");

            var filePaths = dbDump.PerformDBDump();
            var sqlDumpFileName = filePaths.SingleOrDefault(x => x.EndsWith("sql"));

            Publish dbPublish = new Publish(new PublishContext(ConfigurationManager.ConnectionStrings["local"].ConnectionString), sqlDumpFileName);

        }
    }
}
