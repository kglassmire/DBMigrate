using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMigrate
{
    public class DbDiffContext
    {
        public ConnectionStringParts ConnectionStringParts { get; set; }
        public string Name { get; set; }        
        public int VersionNumber { get; set; }

        public bool IsValid => !String.IsNullOrEmpty(Name) && VersionNumber > 0 && ConnectionStringParts.IsValid;
    }
}
