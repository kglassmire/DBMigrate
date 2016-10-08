using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace DBMigrate
{
    public class DBDump
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ConnectionStringParts _localConnectionStringParts;
        private ConnectionStringParts _remoteConnectionStringParts;
        private string _postgresSQLPath;

        public DBDump()
        {
            _localConnectionStringParts = new ConnectionStringParts(ConfigurationManager.ConnectionStrings["local"].ConnectionString);
            _remoteConnectionStringParts = new ConnectionStringParts(ConfigurationManager.ConnectionStrings["remote"].ConnectionString);

            _postgresSQLPath = Path.Combine(ConfigurationManager.AppSettings["PostgreSQLPath"],
                ConfigurationManager.AppSettings["PostgreSQLExecutable"]);
        }

        public void PerformDBDump()
        {
            string _localDumpFileName = String.Format("{0:yyMMddHHmmss}_localDump.backup", DateTime.Now);
            string _localDumpFileNameSQL = String.Format("{0:yyMMddHHmmss}_localDump.sql", DateTime.Now);
            string _remoteDumpFileName = String.Format("{0:yyMMddHHmmss}_remoteDump.backup", DateTime.Now);
            string _remoteDumpFileNameSQL = String.Format("{0:yyMMddHHmmss}_remoteDump.sql", DateTime.Now);

            logger.Info("Starting local raw backup.");
            StartPGDumpProcess(_localConnectionStringParts, _localDumpFileName, DumpType.FullBackup);
            logger.Info("Completed local raw backup.");
            logger.Info("Starting local sql backup.");
            StartPGDumpProcess(_localConnectionStringParts, _localDumpFileNameSQL, DumpType.SQLBackup);
            logger.Info(("Completed local sql backup."));
            logger.Info("Starting remote raw backup.");
            StartPGDumpProcess(_remoteConnectionStringParts, _remoteDumpFileName, DumpType.FullBackup);
            logger.Info("Completed remote raw backup.");
            logger.Info("Starting remote sql backup.");
            StartPGDumpProcess(_remoteConnectionStringParts, _remoteDumpFileNameSQL, DumpType.SQLBackup);
            logger.Info("Completed remote sql backup.");
        }

        private bool IsValidConfiguration()
        {
            bool isValidConfig = true;

            if (_localConnectionStringParts.IsValid)
                return false;
            if (_remoteConnectionStringParts.IsValid)
                return false;
            if (!File.Exists(_postgresSQLPath))
                return false;

            return isValidConfig;
        }

        private void StartPGDumpProcess(ConnectionStringParts connectionStringParts, string outputFile, DumpType dumpType)
        {
            string arguments = GetArgumentsForDumpType(dumpType, connectionStringParts, outputFile);
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = _postgresSQLPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Environment.CurrentDirectory,
                Arguments = arguments                
            };

            if (!String.IsNullOrEmpty(connectionStringParts.Password))
            {
                startInfo.EnvironmentVariables.Add("PGPASSWORD", connectionStringParts.Password);
            }            

            var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += new DataReceivedEventHandler(LogOutput);
            process.ErrorDataReceived += new DataReceivedEventHandler(LogOutput);

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();
        }

        private static void LogOutput(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                logger.Info(e.Data);
            }
        }

        private string GetArgumentsForDumpType(DumpType dumpType, ConnectionStringParts connectionStringParts, string outputFile)
        {
            string arguments = String.Empty;
            switch (dumpType)
            {
                case DumpType.FullBackup:
                    arguments = string.Format("-h {0} -p {1} -U {2} -F c -b -v -f {3} -s {4}",
                        connectionStringParts.Host,
                        connectionStringParts.Port, connectionStringParts.User, outputFile,
                        connectionStringParts.Database);
                    break;
                case DumpType.SQLBackup:
                    arguments = string.Format("-h {0} -p {1} -U {2} -F p -b -v -f {3} -s {4}", 
                        connectionStringParts.Host,
                        connectionStringParts.Port, connectionStringParts.User, outputFile, 
                        connectionStringParts.Database);
                    break;
            }

            return arguments;
        }
    }

    public enum DumpType
    {
        FullBackup = 1,
        SQLBackup = 2
    }
}
