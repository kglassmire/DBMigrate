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
            string _localDumpFileName = String.Format("localDump{0:yyMMddHHmmss}.backup", DateTime.Now);
            string _localDumpFileNameSQL = String.Format("localDump{0:yyMMddHHmmss}.sql", DateTime.Now);
            string _remoteDumpFileName = String.Format("remoteDump{0:yyMMddHHmmss}.backup", DateTime.Now);
            string _remoteDumpFileNameSQL = String.Format("remoteDump{0:yyMMddHHmmss}.sql", DateTime.Now);

            StartPGDumpProcess(_localConnectionStringParts, _localDumpFileName, DumpType.FullBackup);
            StartPGDumpProcess(_localConnectionStringParts, _localDumpFileNameSQL, DumpType.SQLBackup);
            //StartPGDumpProcess(_remoteConnectionStringParts, _remoteDumpFileName, DumpType.FullBackup);
            //StartPGDumpProcess(_remoteConnectionStringParts, _remoteDumpFileNameSQL, DumpType.SQLBackup);
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
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WorkingDirectory = Environment.CurrentDirectory,
                Arguments = arguments                
            };

            if (!String.IsNullOrEmpty(connectionStringParts.Password))
            {
                startInfo.EnvironmentVariables.Add("PGPASSWORD", connectionStringParts.Password);
            }            

            var process = new Process { StartInfo = startInfo };

            process.Start();

            var readerOutput = process.StandardOutput;
            var readerError = process.StandardError;
            //while (!readerOutput.EndOfStream)
            //{
            //    logger.Info(readerOutput.ReadLine());                                
            //}

            process.WaitForExit();
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
