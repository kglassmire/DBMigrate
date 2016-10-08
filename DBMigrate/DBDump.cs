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
    public class DbDump
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private ConnectionStringParts _connectionStringParts;
        private string _connectionStringName;
        private string _postgresSQLPath;

        public DbDump(String connectionString, String connectionStringName)
        {
            _connectionStringName = connectionStringName;
            _connectionStringParts = new ConnectionStringParts(connectionString);
            _postgresSQLPath = Path.Combine(ConfigurationManager.AppSettings["PostgreSQLPath"],
                ConfigurationManager.AppSettings["PostgreSQLExecutable"]);
        }

        public void PerformDBDump()
        {
            string dumpFileName = String.Format("{0:yyMMddHHmmss}_localDump.backup", DateTime.Now);
            string dumpFileNameSQL = String.Format("{0:yyMMddHHmmss}_localDump.sql", DateTime.Now);

            StartPGDumpProcess(_connectionStringParts, dumpFileName, DumpType.FullBackup);
            StartPGDumpProcess(_connectionStringParts, dumpFileNameSQL, DumpType.SQLBackup);
        }

        private bool IsValidConfiguration()
        {
            bool isValidConfig = true;

            if (_connectionStringParts.IsValid)
                return false;
            if (!File.Exists(_postgresSQLPath))
                return false;

            return isValidConfig;
        }

        private void StartPGDumpProcess(ConnectionStringParts connectionStringParts, string outputFile, DumpType dumpType)
        {
            logger.Info("Started backup {0}.", outputFile);

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

            logger.Info("Completed backup {0}.", outputFile);
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
