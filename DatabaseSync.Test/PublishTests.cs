using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace DatabaseSync.Test
{
    [TestFixture]
    public class PublishTests
    {        
        [Test]  
        public void CheckExistenceOfSchemaReturnsBoolean()
        {
            PublishContext context = new PublishContext("User ID=postgres;Password=development;Host=localhost;Port=5432;Database=site;");
            Publish publish = new Publish(context);

            publish.DetermineCurrentSchemaVersion();
        }
    }
}
