using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppConfig.Database.Reflection
{
    public class TableInfo
    {
        public Type TableType { get; private set; }
        public string SchemaName { get; private set; }
        public string TableName { get; private set; }

        public List<ColumnInfo> Keys { get; private set; }
        public List<ColumnInfo> Columns { get; private set; }
    }
}
