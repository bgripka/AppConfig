using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AppConfig.Database.Reflection
{
    public class ColumnInfo
    {
        public PropertyInfo ColumnProperty { get; private set; }
        public string ColumnName { get; private set; }
        public int ColumnOrder { get; private set; }

        public Type PropertyType { get; private set; }
    }
}
