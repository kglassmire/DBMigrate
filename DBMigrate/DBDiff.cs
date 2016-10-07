using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace DBMigrate
{
    public class DBDiff
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _apgDiffPath;

        public DBDiff()
        {
            _apgDiffPath = Path.Combine(ConfigurationManager.AppSettings["APGDiffPath"],
                ConfigurationManager.AppSettings["APGDiffExecutable"]);

            ValidateConfiguration();
            PerformDBDiff();
        }

        private void ValidateConfiguration()
        {
            throw new NotImplementedException();
        }

        private void PerformDBDiff()
        {

        }
    }
}

