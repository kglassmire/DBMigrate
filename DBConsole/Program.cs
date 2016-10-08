using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBMigrate;
using NLog;

namespace DBConsole
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {            
            Dump dbDump = new Dump(ConfigurationManager.ConnectionStrings["local"].ConnectionString, 
                ConfigurationManager.ConnectionStrings["local"].Name);
            logger.Info("Beginning DBDump");
            dbDump.PerformDBDump();
        }
    }
}
