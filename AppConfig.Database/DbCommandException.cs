using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AppConfig.Database
{
    public class DbCommandException : Exception
    {
        public DbCommandException(IDbCommand Command, Exception InnerException)
            : base("Execution of a Database Command Failed.  See InnerException for Details.", InnerException)
        {
            this.CommandText = Command.CommandText;
        }

        public string CommandText { get; private set; }
        
    }
}
