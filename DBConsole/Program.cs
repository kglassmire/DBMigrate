using System;
using System.Collections.Generic;
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
            DBDump dbDump = new DBDump();
            dbDump.PerformDBDump();
        }
    }
}
