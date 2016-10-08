using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSync
{
    public class SchemaVersion
    {
        public int Id { get; set; }
        public string SchemaDump { get; set; }
        public DateTime Timestamp { get; set; }
        public string AssemblyVersion { get; set; }
    }
}
