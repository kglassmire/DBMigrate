﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace DBMigrate
{
    public class DbDiff
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _apgDiffPath;
        private DbDiffContext _sourceContext;
        private DbDiffContext _destinationContext;

        public DbDiff(DbDiffContext sourceContext, DbDiffContext destinationContext)
        {
            _sourceContext = sourceContext;
            _destinationContext = destinationContext;
            _apgDiffPath = Path.Combine(ConfigurationManager.AppSettings["APGDiffPath"],
                ConfigurationManager.AppSettings["APGDiffExecutable"]);

            PerformDBDiff();
        }

        private bool ValidConfiguration(bool sourceSuccess, bool destinationSuccess)
        {
            if (!(sourceSuccess && destinationSuccess))
                return false;
            if (!_sourceContext.IsValid)
                return false;
            if (!_destinationContext.IsValid)
                return false;

            return true;
        }

        private void StartAPGDiffProcess()
        {

        }

        private void PerformDBDiff()
        {
            bool sourceSuccess = CreateFilesForDiff(_sourceContext);
            bool destinationSuccess = CreateFilesForDiff(_destinationContext);

            if (ValidConfiguration(sourceSuccess, destinationSuccess))
            {
            }
        }

        private bool CreateFilesForDiff(DbDiffContext context)
        {
            bool success = false;
            try
            {

            }
            catch (Exception exception)
            {
                logger.Error(exception, "Unable to create file for diff comparison. Diff context {0}", context.Name);
            }

            return success;
        }
    }
}

